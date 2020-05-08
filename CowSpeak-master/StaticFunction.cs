using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Net;

namespace CowSpeak
{
	internal class StaticFunction : FunctionBase
	{
		public MethodInfo Definition;

		public StaticFunction(string Name, MethodInfo Definition, Type type, Parameter[] Parameters, bool isMethod = false) {
			DefinitionType = DefinitionType.Static;
			this.type = type;
			this.Definition = Definition;

			string Params = "";
			for (int i = 0; i < Parameters.Length; i++)
			{
				Params += Parameters[i].Type + " " + Parameters[i].Name;
				if (i != Parameters.Length - 1)
					Params += ", ";
			}

			this.Name = Name;
			this.Parameters = Parameters;
			this.isMethod = isMethod;
		}

		public override Any Execute(string usage)
		{
			if (usage.IndexOf("(") == -1 || usage.IndexOf(")") == -1)
				throw new Exception("Invalid usage of function: " + usage);

			string usage_temp = usage;
			usage = usage.Substring(usage.IndexOf("(")); // reduce it to parentheses and params inside of them
			List< Any > parameters = ParseParameters(usage).ToList();

			Variable caller = null;
			if (isMethod && Parameters.Length != parameters.Count - 1)
			{
				if (usage_temp.IndexOf(".") == -1)
					throw new Exception(Name + " can only be called as a method");

				caller = CowSpeak.Vars[usage_temp.Substring(0, usage_temp.IndexOf("."))];
			}

			CheckParameters(parameters);

			try
			{
				CowSpeak.StackTrace.Add(Usage);

				List<object> InvocationParams = new List<object>();
				if (isMethod)
					InvocationParams.Add(caller);
				foreach (var parameter in parameters)
					InvocationParams.Add(parameter.Value);
				
				object returnValue = Definition.Invoke(null, InvocationParams.ToArray()); // obj is null because the function should be static
				CowSpeak.StackTrace.RemoveAt(CowSpeak.StackTrace.Count - 1);

				if (returnValue == null)
					return null; // Probably a void function

				Type returnedType = null;

				if (returnValue is Any)
					return (Any)returnValue;
				else if (returnValue is System.Array)
					returnedType = Type.GetType(((System.Array)returnValue).GetType());
				else
					returnedType = Type.GetType(returnValue.GetType());

				return new Any(returnedType, returnValue);
			}
			catch (TargetInvocationException ex)
			{
				System.Exception baseEx = ex.GetBaseException();
				if (baseEx is Exception)
					throw baseEx as Exception;
					
				throw baseEx;
			}
		}

		internal static class Functions
		{
			#region ALL_METHODS
			public static string _ToString(object obj)
			{
				if (obj is bool)
					return (bool)obj ? "true" : "false";

				if (obj.GetType().IsArray)
				{
					string ret = "{";
					System.Array Items = (System.Array)obj;

					foreach (object Item in Items)
					{
						ret += (Item != null ? Item.ToString() : "") + ", ";
						System.Console.WriteLine("jeff");
					}

					if (Items.Length > 0)
						ret = ret.Substring(0, ret.LastIndexOf(", "));

					ret += "}";
					return ret;
				}

				return obj.ToString();
			}

			[Method("Any.ToString")]
			public static string ToString(Variable obj)
			{
				return _ToString(obj.Value);
			}

			[Method("Any.Delete")]
			public static void Delete(Variable obj) => CowSpeak.Vars.Remove(obj.Name);
			#endregion

			#region ARRAY_METHODS
			[Method("Array.Length")]
			public static int ArrayLength(Variable obj) => ((System.Array)obj.Value).Length;

			[Method("Array.Set")]
			public static void Set(Variable obj, int index, object value)
			{
				var Value = (System.Array)obj.Value;

				if (index < 0 || index >= Value.Length)
					throw new Exception("Index is out of array bounds");

				Value.SetValue(value, index);
				obj.Value = Value;
			}

			[Method("Array.Get")]
			public static object Get(Variable obj, int index)
			{
				var Value = ((System.Array)obj.Value);

				if (index < 0 || index >= Value.Length)
					throw new Exception("Index is out of array bounds");

				return Value.GetValue(index);
			}
			#endregion

			[Function("Array")]
			public static System.Array Array(int Length, string TypeName) // creates an array based on the type
			{
				if (Length < 0)
					throw new Exception("Array length must be greater than or equal to zero");

				return System.Array.CreateInstance(Type.GetType(TypeName, true).rep, Length);
			}

			#region STRING_METHODS
			static void VerifyParams(int index, int length = 0)
			{
				if (index < 0)
					throw new Exception("Index cannot be less than zero");

				if (length < 0)
					throw new Exception("Length cannot be less than zero");
			}

			[Method(Syntax.Types.String + ".OccurrencesOf")]
			public static int OccurrencesOf(Variable obj, string counter) => Utils.OccurrencesOf(obj.Value.ToString(), counter);

			[Method(Syntax.Types.String + ".Sub" + Syntax.Types.c_String)]
			public static string SubString(Variable obj, int index, int length)
			{
				VerifyParams(index, length);

				string str = obj.Value.ToString();

				if (index + length >= str.Length)
					throw new Exception("Index and length must refer to a location within the string");

				return str.Substring(index, length);
			}

			[Method(Syntax.Types.String + "." + Syntax.Types.c_Character + "At")]
			public static char CharacterAt(Variable obj, int index)
			{
				string str = obj.Value.ToString();

				if (index < 0 || index >= str.Length)
					throw new Exception("Index must refer to a location within the string");

				return str[index];
			}

			[Method(Syntax.Types.String + ".Length")]
			public static int StringLength(Variable obj) => obj.Value.ToString().Length;

			[Method(Syntax.Types.String + ".Remove")]
			public static string Remove(Variable obj, int index, int length)
			{
				string str = obj.Value.ToString();

				VerifyParams(index, length);

				if (index >= str.Length)
					throw new Exception("Index must refer to a location within the string");

				if (index + length >= str.Length)
					throw new Exception("Index and length must refer to a location within the string");

				return str.Remove(index, length);
			}

			[Method(Syntax.Types.String + ".Insert")]
			public static string Insert(Variable obj, int index, string value)
			{
				VerifyParams(index);

				string str = obj.Value.ToString();

				if (index >= str.Length)
					throw new Exception("Index must refer to a location within the string");

				return str.Insert(index, value);
			}

			[Method(Syntax.Types.String + ".IndexOf")]
			public static int IndexOf(Variable obj, string value) => obj.Value.ToString().IndexOf(value);

			[Method(Syntax.Types.String + ".LastIndexOf")]
			public static int LastIndexOf(Variable obj, string value) => obj.Value.ToString().LastIndexOf(value);

			[Method(Syntax.Types.String + ".To" + Syntax.Types.c_Integer)]
			public static int ToInteger(Variable obj)
			{
				int o;
				string str = obj.Value.ToString();

				if (Utils.IsHexadecimal(str))
					return int.Parse(str.Substring(2), System.Globalization.NumberStyles.HexNumber);

				if (System.Int32.TryParse(str, out o))
					return o;
				else
					throw new Exception("Could not convert " + str + " to an " + Syntax.Types.Integer);
			}

			[Method(Syntax.Types.String + ".To" + Syntax.Types.c_Decimal)]
			public static double ToDecimal(Variable obj)
			{
				double o;
				string str = obj.Value.ToString();
				if (System.Double.TryParse(str, out o))
					return o;
				else
					throw new Exception("Could not convert " + str + " to a " + Syntax.Types.Decimal);
			}
			#endregion

			#region CHARACTER_METHODS
			[Method(Syntax.Types.Character + ".ToUpper")]
			public static char ToUpper(Variable obj) => System.Char.ToUpper((char)obj.Value);

			[Method(Syntax.Types.Character + ".ToLower")]
			public static char ToLower(Variable obj) => System.Char.ToLower((char)obj.Value);

			[Method(Syntax.Types.Character + ".ToInteger")]
			public static int CharacterToInteger(Variable obj) => (int)((char)obj.Value);
			#endregion

			#region INTEGER_METHODS
			[Method(Syntax.Types.Integer + ".ToHexadecimal")]
			[Method(Syntax.Types.Integer64 + ".ToHexadecimal")]
			public static string ToHexadecimal(Variable obj)
			{
				return (obj.Value is int ? (int)obj.Value : (long)obj.Value).ToString("X");
			}

			[Method(Syntax.Types.Integer + ".ToCharacter")]
			[Method(Syntax.Types.Integer64 + ".ToCharacter")]
			public static char ToCharacter(Variable obj)
			{
				try
				{
					return (char)(obj.Value is int ? (int)obj.Value : (long)obj.Value);
				}
				catch (System.InvalidCastException)
				{
					throw new Exception("Could not convert " + ((long)obj.Value) + " to a " + Syntax.Types.Character);
				}
			}
			#endregion

			public static void Loop(List<Line> ContainedLines, int i, int CurrentLineOffset, bool isNestedInFunction, string IndexVarName, int StartIndex, int EndIndex)
			{
				Variable iterator = CowSpeak.Vars.Create(new Variable(Type.Integer, IndexVarName));

				for (long p = StartIndex; p < EndIndex; p++)
				{
					Scope scope = new Scope();

					iterator.Value = p;

					Executor.Execute(ContainedLines, i + 1 + CurrentLineOffset, isNestedInFunction, true);

					scope.End();
				}

				CowSpeak.Vars.Remove(iterator.Name); // delete the variable after loop is done
			}

			[Function("InvokeMethod")]
			public static object InvokeMethod(string TypeName, string MethodName, object[] parameters)
			{
				var type = System.Type.GetType(TypeName);

				if (type == null)
					throw new Exception("Cannot find C# type: '" + TypeName + "'");

				var method = type.GetMethod(MethodName, parameters.Select(x => x.GetType()).ToArray());

				if (method == null)
					throw new Exception("Cannot find method '" + MethodName + "' from C# type '" + TypeName + "'");

				if (!method.IsStatic)
					throw new Exception("Cannot invoke non-static method: '" + MethodName + "'");

				Type.GetType(method.ReturnType); // throw an exception if the method's return type isn't a valid CowSpeak type

				return method.Invoke(null, parameters);
			}

			[Function("GetCurrentSeconds")]
			public static double GetCurrentSeconds() => System.DateTime.Now.Second;

			[Function("GetCurrentMilliseconds")]
			public static double GetCurrentMilliseconds() => System.DateTime.Now.Millisecond;

			[Function("ReadFileLines")]
			public static string[] ReadFileLines(string filePath) => File.ReadAllLines(filePath);

			[Function("WriteFileLines")]
			public static void WriteFileLines(string filePath, string[] lines) => File.WriteAllLines(filePath, lines);

			[Function("DeleteFile")]
			public static void DeleteFile(string filePath) => File.Delete(filePath);

			[Function("DoesFileExist")]
			public static bool DoesFileExist(string filePath) => File.Exists(filePath);

			[Function("Sin")]
			public static double Sin(double num) => System.Math.Sin(num);

			[Function("Cos")]
			public static double Cos(double num) => System.Math.Cos(num);

			[Function("Tan")]
			public static double Tan(double num) => System.Math.Tan(num);

			[Function("Run")]
			public static void Run(string filePath)
			{
				string oldFile = string.Copy(CowSpeak.CurrentFile);

				// make modulePath relative to CurrentFile as long as modulePath is relative
				if (!Path.IsPathRooted(filePath))
				{
					if (CowSpeak.CurrentFile.IndexOf("/") != -1)
						filePath = CowSpeak.CurrentFile.Substring(0, CowSpeak.CurrentFile.IndexOf("/") + 1) + filePath;
					if (CowSpeak.CurrentFile.IndexOf("\\") != -1)
						filePath = CowSpeak.CurrentFile.Substring(0, CowSpeak.CurrentFile.IndexOf("\\") + 1) + filePath;
				}

				CowSpeak.Run(filePath);
				CowSpeak.CurrentFile = oldFile;
			}

			[Function("ExecuteFile")]
			public static void ExecuteFile(string filePath)
			{
				string oldFile = string.Copy(CowSpeak.CurrentFile);

				// make modulePath relative to CurrentFile as long as modulePath is relative
				if (!Path.IsPathRooted(filePath))
				{
					if (CowSpeak.CurrentFile.IndexOf("/") != -1)
						filePath = CowSpeak.CurrentFile.Substring(0, CowSpeak.CurrentFile.IndexOf("/") + 1) + filePath;
					if (CowSpeak.CurrentFile.IndexOf("\\") != -1)
						filePath = CowSpeak.CurrentFile.Substring(0, CowSpeak.CurrentFile.IndexOf("\\") + 1) + filePath;
				}

				CowSpeak.Execute(filePath);
				CowSpeak.CurrentFile = oldFile;
			}

			[Function("GetHtmlFromUrl")]
			public static string GetHtmlFromUrl(string url)
			{
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
				HttpWebResponse response = (HttpWebResponse)request.GetResponse();

				if (response.StatusCode == HttpStatusCode.OK)
				{
					Stream receiveStream = response.GetResponseStream();
					StreamReader readStream = null;

					if (string.IsNullOrWhiteSpace(response.CharacterSet))
						readStream = new StreamReader(receiveStream);
					else
						readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));

					string data = readStream.ReadToEnd();

					response.Close();
					readStream.Close();
					return data;
				}

				throw new Exception("Cannot get a HttpWebResponse from '" + url + "'");
			}

			[Function("Abs")]
			public static double Abs(double value) => System.Math.Abs(value);

			[Function("Round")]
			public static double Round(double value) => System.Math.Round(value);

			[Function("Sqrt")]
			public static double Sqrt(double value) => System.Math.Sqrt(value);

			[Function("Pow")]
			public static double Pow(double x, double y) => System.Math.Pow(x, y);

			[Function("Sleep")]
			public static void Sleep(int ms) => Thread.Sleep(ms);

			[Function("InputKey")]
			public static char ReadCharacter() => (char)System.Console.ReadKey().KeyChar;

			[Function("ClearConsole")]
			public static void ClearConsole() => System.Console.Clear();

			[Function("Define")]
			public static void Define(string from, string to)
			{
				CowSpeak.Definitions.Add(new Definition
				{
					from = from,
					to = to,
					DefinitionType = DefinitionType.User
				});
			}

			[Function("Evaluate")]
			public static object _Evaluate(string Expression) => Utils.Eval(Expression);

			[Function("Random" + Syntax.Types.c_Integer)]
			public static int RandomInteger(int minimum, int maximum)
			{
				if (minimum > maximum)
					throw new Exception("Minimum must be less than the maximum");

				return Utils.rand.Next(minimum, maximum);
			}

			[Function("Print")]
			public static void Print(object text) => System.Console.Write(_ToString(text));

			[Function("Exit")]
			public static void Exit(int exitCode) => System.Environment.Exit(exitCode);

			[Function("ThrowError")]
			public static void ThrowError(string errorText) => throw new Exception(errorText);

			[Function("Input" + Syntax.Types.c_String)]
			public static string InputString() => System.Console.ReadLine();

			[Function("Input" + Syntax.Types.c_Character)]
			public static char InputCharacter() => System.Console.ReadKey().KeyChar;
		}
	}

	[System.AttributeUsage(System.AttributeTargets.Method)]
	internal class FunctionAttribute : System.Attribute
	{
		public string Name;

		public FunctionAttribute(string Name)
		{
			this.Name = Name;
		}

		public static Functions GetFunctions()
		{
			Functions functions = new Functions();

			var methods = typeof(StaticFunction.Functions).GetMethods().Where(m => m.GetCustomAttributes(typeof(FunctionAttribute), false).Length > 0 || m.GetCustomAttributes(typeof(MethodAttribute), false).Length > 0); // Get all methods from the function class with the function attributes

            foreach (MethodInfo method in methods)
			{
				if (method == null)
					continue;

				FunctionAttribute functionAttr = ((FunctionAttribute[])GetCustomAttributes(method, typeof(FunctionAttribute))).Where(attr => !(attr is MethodAttribute)).FirstOrDefault(); // get attribute for function
				MethodAttribute[] methodAttrs = (MethodAttribute[])GetCustomAttributes(method, typeof(MethodAttribute)); // get attribute for method

				string Name;
				bool isMethod = false;
				if (methodAttrs.Length > 0)
				{
					Name = methodAttrs[0].Name;
					isMethod = true;
				}
				else if (functionAttr != null)
					Name = functionAttr.Name;
				else
					continue; // skip method, it does not have either attribute   

				List<Parameter> parameters = new List<Parameter>();
				foreach (var parameter in method.GetParameters())
				{
					var paramType = Type.GetType(parameter.ParameterType, false);
					if (paramType == null)
						continue;

					parameters.Add(new Parameter(paramType, parameter.Name));
				}

				var returnType = Type.GetType(method.ReturnType, false);
				if (returnType == null)
					returnType = Type.Any;

                functions.Add(Name, new StaticFunction(Name, method, returnType, parameters.ToArray(), isMethod));
				if (methodAttrs.Length > 1)
				{
					// Add the function again but with the additional MethodAttribute's name(s)
					for (int i = 1; i < methodAttrs.Length; i++)
						functions.Add(methodAttrs[i].Name, new StaticFunction(Name, method, returnType, parameters.ToArray(), isMethod));
				}
			}

            return functions;
        } // get a list of all methods from the Functions class that have this attribute
	}

	[System.AttributeUsage(System.AttributeTargets.Method, AllowMultiple = true)]
	internal class MethodAttribute : FunctionAttribute
	{
		public MethodAttribute(string Name) : base(Name){}
	}
}