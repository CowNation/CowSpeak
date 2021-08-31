using CowSpeak.Exceptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;

namespace CowSpeak.Modules
{
	[ModuleAttribute.AutoImport]
	[Module("Main")]
	public static class Main
	{
		public static string _ToString(object obj)
		{
			if (obj == null)
				return "null";

			if (obj is bool)
				return (bool)obj ? "true" : "false";

			if (obj is IList)
			{
				string ret = "{";
				IList items = (IList)obj;
				
				foreach (object item in items)
				{
					bool isVariable = item.GetType() == typeof(Variable);
					bool isString = item.GetType() == typeof(string);
					bool isChar = item.GetType() == typeof(char);

					string itemStr = "";

					if (isVariable)
                    {
						var variable = (Variable)item;
						var varName = variable.Name;

						if (varName.Contains(".") && varName.LastIndexOf(".") + 1 < varName.Length)
							varName = varName.Substring(varName.LastIndexOf(".") + 1);

						// surround the value in "s or 's if it's a string or char
						// _ToString only does that for elements of ILists so we have to do it here
						var valueText = _ToString(variable.Value);
						if (variable.Type.CSharpType == typeof(string))
							valueText = "\"" + valueText + "\"";
						else if (variable.Type.CSharpType == typeof(char))
							valueText = "\'" + valueText + "\'";

						itemStr = string.Format("{0}: {1}", varName, valueText);
					}
                    else
                    {
						itemStr = (item != null ? item.ToString() : "");

						if (isString)
							itemStr = "\"" + itemStr + "\"";
						else if (isChar)
							itemStr = "\'" + itemStr + "\'";
					}


					ret += itemStr + ", ";
				}

				if (items.Count > 0)
					ret = ret.Substring(0, ret.LastIndexOf(", "));

				ret += "}";
				return ret;
			}

			return obj.ToString();
		}

		[Method(Syntax.Types.c_Byte + Syntax.Types.ArraySuffix + ".GetString")]
		public static string GetString(Variable obj, string encoding)
		{
			encoding = encoding.Replace("-", "");

			if (encoding == "UTF16")
				encoding = "Unicode";

			var encodingType = typeof(System.Text.Encoding).GetProperty(encoding);
			if (encodingType == null)
				throw new BaseException("Unknown encoding type: " + encoding);

			return (string)typeof(System.Text.Encoding)
				.GetMethod("GetString", new System.Type[] { typeof(byte[]) })
				.Invoke(encodingType.GetValue(null), new object[] { (byte[])obj.Value });
		}

		[Method(Syntax.Types.c_Object + Syntax.Types.ArraySuffix + ".Create", true)]
		[Method(Syntax.Types.c_Decimal + Syntax.Types.ArraySuffix + ".Create", true)]
		[Method(Syntax.Types.c_Boolean + Syntax.Types.ArraySuffix + ".Create", true)]
		[Method(Syntax.Types.c_Character + Syntax.Types.ArraySuffix + ".Create", true)]
		[Method(Syntax.Types.c_Integer + Syntax.Types.ArraySuffix + ".Create", true)]
		[Method(Syntax.Types.c_String + Syntax.Types.ArraySuffix + ".Create", true)]
		[Method(Syntax.Types.c_Byte + Syntax.Types.ArraySuffix + ".Create", true)]
		public static Array CreateArray(Type type, int length)
		{
			return Array.CreateInstance(type.CSharpType.GetElementType(), length);
		}

        #region CHARACTER_METHODS
        [Method(Syntax.Types.Character + ".ToUpper")]
		public static char ToUpper(Variable obj) => char.ToUpper((char)obj.Value);

		[Method(Syntax.Types.Character + ".ToLower")]
		public static char ToLower(Variable obj) => char.ToLower((char)obj.Value);
		#endregion

		#region BOOLEAN_METHODS
		[Method(Syntax.Types.Boolean + ".Flip")]
        public static bool Flip(Variable obj) => !(bool)obj.Value;
		#endregion

        public static void Loop(List<Line> containedLines, int i, int currentLineOffset, bool isNestedInFunction, string indexVarName, int startIndex, int endIndex)
		{
			Variable iterator = Interpreter.Vars.Create(new Variable(Types.Integer, indexVarName));

			try
			{
				for (long p = startIndex; p < endIndex; p++)
				{
					Scope scope = new Scope();

					iterator.Value = p;

					Executor.Execute(containedLines, i + 1 + currentLineOffset, isNestedInFunction, true);

					scope.End();
				}
			}
			catch (Exception ex)
			{
				Interpreter.Vars.Remove(iterator.Name);

				if (ex is BaseException)
					throw ex;
				else
					throw new BaseException(ex.Message);
			}

			Interpreter.Vars.Remove(iterator.Name); // delete the variable after loop is done
		}

		[Function("InvokeStaticMethod")]
		public static object InvokeStaticMethod(string typeName, string methodName, object[] parameters)
		{
			var type = System.Type.GetType(typeName);

			if (type == null)
				throw new BaseException("Cannot find .NET type: '" + typeName + "'");

			var method = type.GetMethod(methodName, parameters.Select(x => x.GetType()).ToArray());

			if (method == null)
				throw new BaseException("Cannot find method '" + methodName + "' from .NET type '" + typeName + "'");

			if (!method.IsStatic)
				throw new BaseException("Cannot invoke non-static method: '" + methodName + "'");

			return method.Invoke(null, parameters);
		}

		[Function]
		public static bool GetDebug() => Interpreter.Debug;
		[Function]
		public static string GetCurrentFile() => Interpreter.CurrentFile;
		[Function]
		public static int GetCurrentLine() => Interpreter.CurrentLine;

		[Function("GetCurrentSeconds")]
		public static int GetCurrentSeconds() => DateTime.Now.Second;

		[Function("GetCurrentMilliseconds")]
		public static int GetCurrentMilliseconds() => DateTime.Now.Millisecond;

		[Function("ReadFileLines")]
		public static string[] ReadFileLines(string filePath) => File.ReadAllLines(filePath);

		[Function("WriteFileLines")]
		public static void WriteFileLines(string filePath, string[] lines) => File.WriteAllLines(filePath, lines);

		[Function("DeleteFile")]
		public static void DeleteFile(string filePath) => File.Delete(filePath);

		[Function("DoesFileExist")]
		public static bool DoesFileExist(string filePath) => File.Exists(filePath);

		[Function("Sin")]
		public static double Sin(double num) => Math.Sin(num);

		[Function("Cos")]
		public static double Cos(double num) => Math.Cos(num);

		[Function("Tan")]
		public static double Tan(double num) => Math.Tan(num);

		[Function("Run")]
		public static void Run(string filePath)
		{
			string oldFile = string.Copy(Interpreter.CurrentFile);

			// make modulePath relative to CurrentFile as long as modulePath is relative
			if (!Path.IsPathRooted(filePath))
			{
				if (Interpreter.CurrentFile.IndexOf("/") != -1)
					filePath = Interpreter.CurrentFile.Substring(0, Interpreter.CurrentFile.IndexOf("/") + 1) + filePath;
				if (Interpreter.CurrentFile.IndexOf("\\") != -1)
					filePath = Interpreter.CurrentFile.Substring(0, Interpreter.CurrentFile.IndexOf("\\") + 1) + filePath;
			}

			Interpreter.Run(filePath);
			Interpreter.CurrentFile = oldFile;
		}

		[Function("ExecuteFile")]
		public static void ExecuteFile(string filePath)
		{
			string oldFile = string.Copy(Interpreter.CurrentFile);

			// make modulePath relative to CurrentFile as long as modulePath is relative
			if (!Path.IsPathRooted(filePath))
			{
				if (Interpreter.CurrentFile.IndexOf("/") != -1)
					filePath = Interpreter.CurrentFile.Substring(0, Interpreter.CurrentFile.IndexOf("/") + 1) + filePath;
				if (Interpreter.CurrentFile.IndexOf("\\") != -1)
					filePath = Interpreter.CurrentFile.Substring(0, Interpreter.CurrentFile.IndexOf("\\") + 1) + filePath;
			}

			Interpreter.Execute(filePath);
			Interpreter.CurrentFile = oldFile;
		}

		[Function("GetHtmlFromUrl")]
		public static string GetHtmlFromUrl(string url)
		{
			// C+P'd from SO
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
			HttpWebResponse response = (HttpWebResponse)request.GetResponse();

			if (response.StatusCode == HttpStatusCode.OK)
			{
				Stream receiveStream = response.GetResponseStream();
				StreamReader readStream = null;

				if (string.IsNullOrWhiteSpace(response.CharacterSet))
					readStream = new StreamReader(receiveStream);
				else
					readStream = new StreamReader(receiveStream, System.Text.Encoding.GetEncoding(response.CharacterSet));

				string data = readStream.ReadToEnd();

				response.Close();
				readStream.Close();
				return data;
			}

			throw new BaseException("Cannot get a HttpWebResponse from '" + url + "'");
		}

		[Function("Abs")]
		public static double Abs(double value) => Math.Abs(value);

		[Function("Round")]
		public static double Round(double value) => Math.Round(value);

		[Function("Sqrt")]
		public static double Sqrt(double value) => Math.Sqrt(value);

		[Function("Pow")]
		public static double Pow(double x, double y) => Math.Pow(x, y);

		[Function("Sleep")]
		public static void Sleep(int ms) => Thread.Sleep(ms);

		[Function("InputKey")]
		public static char ReadCharacter() => Console.ReadKey().KeyChar;

		[Function("ClearConsole")]
		public static void ClearConsole() => Console.Clear();

		[Function("Define")]
		public static void Define(string from, string to)
		{
			Interpreter.Definitions.Add(from, new Definition
			{
				From = from,
				To = to,
				DefinitionType = DefinitionType.User
			});
		}

		[Function("EvaluateExpression")]
		public static object EvaluateExpression(string expression) => Utils.Eval(expression);

		[Function("Random" + Syntax.Types.c_Integer)]
		public static int RandomInteger(int minimum, int maximum)
		{
			if (minimum > maximum)
				throw new BaseException("Minimum must be less than the maximum");

			return Utils.rand.Next(minimum, maximum);
		}

		[Function("Print")]
		public static void Print(object text) => Console.Write(_ToString(text));

		[Function("Exit")]
		public static void Exit(int exitCode) => Environment.Exit(exitCode);

		[Function("ThrowError")]
		public static void ThrowError(string errorText) => throw new BaseException(errorText);

		[Function("Input" + Syntax.Types.c_String)]
		public static string InputString() => Console.ReadLine();

		[Function("Input" + Syntax.Types.c_Character)]
		public static char InputCharacter() => Console.ReadKey().KeyChar;
	}
}
