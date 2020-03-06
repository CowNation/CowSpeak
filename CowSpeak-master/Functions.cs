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
		[FunctionAttr("Any.ToString", true)]
		public static string ToString(Variable obj)
		{
			if (obj.Type.rep.IsArray)
			{
				string ret = "{";
				System.Array Items = (System.Array)obj.Value;
				IEnumerator myEnumerator = Items.GetEnumerator();
				while (( myEnumerator.MoveNext() ) && ( myEnumerator.Current != null ))
					ret += myEnumerator.Current.ToString() + ", ";
				if (Items.Length > 0)
					ret = ret.Substring(0, ret.LastIndexOf(", "));
				ret += "}";
				return ret;
			}

			return obj.Value.ToString();
		}
		#endregion

		#region ARRAY_FUNCTIONS
		[FunctionAttr("Array.Length", true)]
		public static int ArrayLength(Variable obj) => ((System.Array)obj.Value).Length;
		#endregion

		#region ARRAY_CONSTRUCTORS
		[FunctionAttr("Byte_Array")]
		public static byte[] ByteArray(int length) => new byte[length];
		#endregion

		#region STRING_METHODS
		[FunctionAttr(Syntax.Types.String + ".OccurrencesOf", true)]
		public static int OccurrencesOf(Variable obj, string counter) => Utils.OccurrencesOf(obj.Value.ToString(), counter);

		[FunctionAttr(Syntax.Types.String + ".Sub" + Syntax.Types.c_String, true)]
		public static string SubString(Variable obj, int index, int length) => obj.Value.ToString().Substring(index, length);

		[FunctionAttr(Syntax.Types.String + "." + Syntax.Types.c_Character + "At", true)]
		public static char CharacterAt(Variable obj, int index) => obj.Value.ToString()[index];

		[FunctionAttr(Syntax.Types.String + ".Length", true)]
		public static int StringLength(Variable obj) => obj.Value.ToString().Length;

		[FunctionAttr(Syntax.Types.String + ".Remove", true)]
		public static string Remove(Variable obj, int index, int length) => obj.Value.ToString().Remove(index, length);

		[FunctionAttr(Syntax.Types.String + ".Insert", true)]
		public static string Insert(Variable obj, int index, string value) => obj.Value.ToString().Insert(index, value);

		[FunctionAttr(Syntax.Types.String + ".IndexOf", true)]
		public static int IndexOf(Variable obj, string value) => obj.Value.ToString().IndexOf(value);

		[FunctionAttr(Syntax.Types.String + ".LastIndexOf", true)]
		public static int LastIndexOf(Variable obj, string value) => obj.Value.ToString().LastIndexOf(value);

		[FunctionAttr(Syntax.Types.String + ".To" + Syntax.Types.c_Integer, true)]
		public static int ToInteger(Variable obj)
		{
			int o;
			string str = obj.Value.ToString();

			if (Utils.IsHexadecimal(str))
				return int.Parse(str.Substring(2), System.Globalization.NumberStyles.HexNumber);

			if (System.Int32.TryParse(str, out o))
				return o;
			else
				throw new Exception("Could not convert " + Syntax.Types.String + " to an " + Syntax.Types.Integer);
		}

		[FunctionAttr(Syntax.Types.String + ".To" + Syntax.Types.c_Decimal, true)]
		public static double ToDecimal(Variable obj)
		{
			double o;
			string str = obj.Value.ToString();
			if (System.Double.TryParse(str, out o))
				return o;
			else
				throw new Exception("Could not convert " + Syntax.Types.String + " to an " + Syntax.Types.Decimal);
		}
		#endregion

		#region CHARACTER_METHODS
		[FunctionAttr(Syntax.Types.Character + ".ToUpper", true)]
		public static char ToUpper(Variable obj) => System.Char.ToUpper((char)obj.Value);

		[FunctionAttr(Syntax.Types.Character + ".ToLower", true)]
		public static char ToLower(Variable obj) => System.Char.ToLower((char)obj.Value);

		[FunctionAttr(Syntax.Types.Character + ".ToInteger", true)]
		public static int CharacterToInteger(Variable obj) => (int)((char)obj.Value);
		#endregion

		#region INTEGER_METHODS
		[FunctionAttr(Syntax.Types.Integer + ".ToHexadecimal", true)]
		public static string ToHexadecimal(Variable obj) => ((int)obj.Value).ToString("X");

		[FunctionAttr(Syntax.Types.Integer + ".ToCharacter", true)]
		public static char ToCharacter(Variable obj) => (char)(int)obj.Value;
		#endregion

		#region INTEGER64_METHODS
		[FunctionAttr(Syntax.Types.Integer64 + ".ToHexadecimal", true)]
		public static string ToHexadecimal64(Variable obj) => ((long)obj.Value).ToString("X");

		[FunctionAttr(Syntax.Types.Integer64 + ".ToCharacter", true)]
		public static char ToCharacter64(Variable obj) => (char)(long)obj.Value;
		#endregion

		[FunctionAttr("Sin")]
		public static double Sin(double num) => System.Math.Sin(num);

		[FunctionAttr("Cos")]
		public static double Cos(double num) => System.Math.Cos(num);

		[FunctionAttr("Tan")]
		public static double Tan(double num) => System.Math.Tan(num);

		[FunctionAttr("Import")]
		public static void Import(string modulePath)
		{
			string oldFile = string.Copy(CowSpeak.CurrentFile);

			// make modulePath relative to CurrentFile as long as modulePath is relative
			if (!Path.IsPathRooted(modulePath))
			{
				if (CowSpeak.CurrentFile.IndexOf("/") != -1)
					modulePath = CowSpeak.CurrentFile.Substring(0, CowSpeak.CurrentFile.IndexOf("/") + 1) + modulePath;
				if (CowSpeak.CurrentFile.IndexOf("\\") != -1)
					modulePath = CowSpeak.CurrentFile.Substring(0, CowSpeak.CurrentFile.IndexOf("\\") + 1) + modulePath;
			}

			CowSpeak.Exec(modulePath);
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

		[FunctionAttr("Read")]
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
		public static object _Evaluate(string toExec) => Evaluate.EvaluateExpression(toExec);

		[FunctionAttr("Random" + Syntax.Types.c_Integer)]
		public static int RandomInteger(int minimum, int maximum)
		{
			if (minimum > maximum)
				throw new Exception("Minimum must be less than the maximum");

			return Utils.rand.Next(minimum, maximum);
		}

		[FunctionAttr("Print")]
		public static void Print(object text) => System.Console.Write(text.ToString());

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