using System.Threading;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using System.IO;
using System.Net;

namespace CowSpeak
{
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
				IEnumerator myEnumerator = Items.GetEnumerator();
				while (( myEnumerator.MoveNext() ) && ( myEnumerator.Current != null ))
					ret += myEnumerator.Current.ToString() + ", ";
				if (Items.Length > 0)
					ret = ret.Substring(0, ret.LastIndexOf(", "));
				ret += "}";
				return ret;
			}

			return obj.ToString();
		}

		[MethodAttr("Any.ToString")]
		public static string ToString(Variable obj)
		{
			return _ToString(obj.Value);
		}

		[MethodAttr("Any.Delete")]
		public static void Delete(Variable obj) => CowSpeak.Vars.Remove(obj);
		#endregion

		#region ARRAY_METHODS
		[MethodAttr("Array.Length")]
		public static int ArrayLength(Variable obj) => ((System.Array)obj.Value).Length;
		
		[MethodAttr("Array.Set")]
		public static void Set(Variable obj, int index, object value)
		{
			var Value = (System.Array)obj.Value;

			if (index < 0 || index >= Value.Length)
				throw new Exception("Index is out of array bounds");

			Value.SetValue(value, index);
			obj.Value = Value;
		}

		[MethodAttr("Array.Get")]
		public static object Get(Variable obj, int index)
		{
			var Value = ((System.Array)obj.Value);

			if (index < 0 || index >= Value.Length)
				throw new Exception("Index is out of array bounds");

			return Value.GetValue(index);
		}
		#endregion

		[FunctionAttr("Array")]
		public static System.Array Array(int length, object initialValue) // creates an array based on initialValue's type
		{
			if (length < 0)
				throw new Exception("Array length must be greater than or equal to zero");

			var arr = System.Array.CreateInstance(initialValue.GetType(), length);
			if (Type.GetType(arr.GetType(), false) == null)
				throw new Exception("Invalid type of object passed as initialValue"); // This may not be possible but better safe then sorry
			for (int i = 0; i < arr.Length; i++)
				arr.SetValue(initialValue, i);
			return arr;
		}

		#region STRING_METHODS
		static void VerifyParams(int index, int length = 0)
		{
			if (index < 0)
				throw new Exception("Index cannot be less than zero");

			if (length < 0)
				throw new Exception("Length cannot be less than zero");
		}

		[MethodAttr(Syntax.Types.String + ".OccurrencesOf")]
		public static int OccurrencesOf(Variable obj, string counter) => Utils.OccurrencesOf(obj.Value.ToString(), counter);

		[MethodAttr(Syntax.Types.String + ".Sub" + Syntax.Types.c_String)]
		public static string SubString(Variable obj, int index, int length)
		{
			VerifyParams(index, length);

			string str = obj.Value.ToString();

			if (index + length >= str.Length)
				throw new Exception("Index and length must refer to a location within the string");

			return str.Substring(index, length);
		}

		[MethodAttr(Syntax.Types.String + "." + Syntax.Types.c_Character + "At")]
		public static char CharacterAt(Variable obj, int index)
		{
			string str = obj.Value.ToString();

			if (index < 0 || index >= str.Length)
				throw new Exception("Index must refer to a location within the string");

			return str[index];
		}

		[MethodAttr(Syntax.Types.String + ".Length")]
		public static int StringLength(Variable obj) => obj.Value.ToString().Length;

		[MethodAttr(Syntax.Types.String + ".Remove")]
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

		[MethodAttr(Syntax.Types.String + ".Insert")]
		public static string Insert(Variable obj, int index, string value)
		{
			VerifyParams(index);

			string str = obj.Value.ToString();

			if (index >= str.Length)
				throw new Exception("Index must refer to a location within the string");

			return str.Insert(index, value);
		}

		[MethodAttr(Syntax.Types.String + ".IndexOf")]
		public static int IndexOf(Variable obj, string value) => obj.Value.ToString().IndexOf(value);

		[MethodAttr(Syntax.Types.String + ".LastIndexOf")]
		public static int LastIndexOf(Variable obj, string value) => obj.Value.ToString().LastIndexOf(value);

		[MethodAttr(Syntax.Types.String + ".To" + Syntax.Types.c_Integer)]
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

		[MethodAttr(Syntax.Types.String + ".To" + Syntax.Types.c_Decimal)]
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
		[MethodAttr(Syntax.Types.Character + ".ToUpper")]
		public static char ToUpper(Variable obj) => System.Char.ToUpper((char)obj.Value);

		[MethodAttr(Syntax.Types.Character + ".ToLower")]
		public static char ToLower(Variable obj) => System.Char.ToLower((char)obj.Value);

		[MethodAttr(Syntax.Types.Character + ".ToInteger")]
		public static int CharacterToInteger(Variable obj) => (int)((char)obj.Value);
		#endregion

		#region INTEGER_METHODS
		[MethodAttr(Syntax.Types.Integer + ".ToHexadecimal")]
		public static string ToHexadecimal(Variable obj) => ((int)obj.Value).ToString("X");

		[MethodAttr(Syntax.Types.Integer + ".ToCharacter")]
		public static char ToCharacter(Variable obj)
		{
			try
			{
				return (char)(int)obj.Value;
			}
			catch (System.InvalidCastException)
			{
				throw new Exception("Could not convert " + ((int)obj.Value) + " to a " + Syntax.Types.Character);
			}
		}
		#endregion

		#region INTEGER64_METHODS
		[MethodAttr(Syntax.Types.Integer64 + ".ToHexadecimal")]
		public static string ToHexadecimal64(Variable obj) => ((long)obj.Value).ToString("X");

		[MethodAttr(Syntax.Types.Integer64 + ".ToCharacter")]
		public static char ToCharacter64(Variable obj)
		{
			try
			{
				return (char)(long)obj.Value;
			}
			catch (System.InvalidCastException)
			{
				throw new Exception("Could not convert " + ((long)obj.Value) + " to a " + Syntax.Types.Character);
			}
		}
		#endregion

		public static void Loop(List<Line> ContainedLines, int i, int CurrentLineOffset, bool isNestedInFunction, string IndexVarName, int StartIndex, int EndIndex)
		{
			CowSpeak.Vars.Create(new Variable(Type.Integer, IndexVarName));

			for (long p = StartIndex; p < EndIndex; p++)
			{
				Scope scope = new Scope();

				CowSpeak.Vars.Get(IndexVarName).Value = p;

				Executor.Execute(ContainedLines, i + 1 + CurrentLineOffset, isNestedInFunction, true);

				scope.End();
			}

			CowSpeak.Vars.Remove(CowSpeak.Vars.Get(IndexVarName)); // delete the variable after loop is done
		}

		[FunctionAttr("GetCurrentSeconds")]
		public static double GetCurrentSeconds() => System.DateTime.Now.Second;

		[FunctionAttr("GetCurrentMilliseconds")]
		public static double GetCurrentMilliseconds() => System.DateTime.Now.Millisecond;

		[FunctionAttr("ReadFileLines")]
		public static string[] ReadFileLines(string filePath) => File.ReadAllLines(filePath);

		[FunctionAttr("WriteFileLines")]
		public static void WriteFileLines(string filePath, string[] lines) => File.WriteAllLines(filePath, lines);

		[FunctionAttr("DeleteFile")]
		public static void DeleteFile(string filePath) => File.Delete(filePath);

		[FunctionAttr("DoesFileExist")]
		public static bool DoesFileExist(string filePath) => File.Exists(filePath);

		[FunctionAttr("Sin")]
		public static double Sin(double num) => System.Math.Sin(num);

		[FunctionAttr("Cos")]
		public static double Cos(double num) => System.Math.Cos(num);

		[FunctionAttr("Tan")]
		public static double Tan(double num) => System.Math.Tan(num);

		[FunctionAttr("Run")]
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

		[FunctionAttr("Exec")]
		public static void Exec(string filePath)
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

			CowSpeak.Exec(filePath);
			CowSpeak.CurrentFile = oldFile;
		}

		[FunctionAttr("GetHtmlFromUrl")]
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

		[FunctionAttr("Abs")]
		public static double Abs(double value) => System.Math.Abs(value);

		[FunctionAttr("Round")]
		public static double Round(double value) => System.Math.Round(value);

		[FunctionAttr("Sqrt")]
		public static double Sqrt(double value) => System.Math.Sqrt(value);

		[FunctionAttr("Pow")]
		public static double Pow(double x, double y) => System.Math.Pow(x, y);

		[FunctionAttr("Sleep")]
		public static void Sleep(int ms) => Thread.Sleep(ms);

		[FunctionAttr("InputKey")]
		public static char ReadCharacter() => (char)System.Console.ReadKey().KeyChar;

		[FunctionAttr("ClearConsole")]
		public static void ClearConsole() => System.Console.Clear();

		[FunctionAttr("Define")]
		public static void Define(string from, string to) 
		{
			CowSpeak.Definitions.Add(new Definition
			{
				from = from,
				to = to,
				DefinitionType = DefinitionType.User
			});
		}

		[FunctionAttr("Evaluate")]
		public static object _Evaluate(string Expression) => Utils.Eval(Expression);

		[FunctionAttr("Random" + Syntax.Types.c_Integer)]
		public static int RandomInteger(int minimum, int maximum)
		{
			if (minimum > maximum)
				throw new Exception("Minimum must be less than the maximum");

			return Utils.rand.Next(minimum, maximum);
		}

		[FunctionAttr("Print")]
		public static void Print(object text) => System.Console.Write(_ToString(text));

		[FunctionAttr("Exit")]
		public static void Exit(int exitCode) => System.Environment.Exit(exitCode);

		[FunctionAttr("ThrowError")]
		public static void ThrowError(string errorText) => throw new Exception(errorText);

		[FunctionAttr("Input" + Syntax.Types.c_String)]
		public static string InputString() => System.Console.ReadLine();

		[FunctionAttr("Input" + Syntax.Types.c_Integer)]
		public static int InputInteger()
		{
			string built = "";
			System.ConsoleKeyInfo key = new System.ConsoleKeyInfo();
			while (key.Key != System.ConsoleKey.Enter)
			{
				Thread.Sleep(100);

				key = System.Console.ReadKey();
				if ((key.KeyChar >= '0' && key.KeyChar <= '9') || (built.IndexOf("-") == -1 && key.KeyChar == '-'))
					built += key.KeyChar;
				else if (key.Key == System.ConsoleKey.Backspace)
					built = built.Remove(built.Length - 1, 1);
				else
					System.Console.Write("\b \b");
			}
			int _out = -1;
			System.Int32.TryParse(built, out _out);
			return _out;
		}

		[FunctionAttr("Input" + Syntax.Types.c_Character)]
		public static char InputCharacter() => System.Console.ReadKey().KeyChar;

		[FunctionAttr("Input" + Syntax.Types.c_Decimal)]
		public static double InputDecimal()
		{
			string built = "";
			System.ConsoleKeyInfo key = new System.ConsoleKeyInfo();
			while (key.Key != System.ConsoleKey.Enter)
			{
				Thread.Sleep(50);

				key = System.Console.ReadKey();
				if ((key.KeyChar >= '0' && key.KeyChar <= '9') || (built.IndexOf(".") == -1 && key.KeyChar == '.') || (built.IndexOf("-") == -1 && key.KeyChar == '-'))
					built += key.KeyChar;
				else if (key.Key == System.ConsoleKey.Backspace)
					built = built.Remove(built.Length - 1, 1);
				else
					System.Console.Write("\b \b");
			}
			float _out = -1;
			System.Single.TryParse(built, out _out);
			return _out;
		}
	}
}