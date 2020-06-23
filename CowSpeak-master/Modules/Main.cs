﻿using CowSpeak.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading;

namespace CowSpeak.Modules
{
	[Module("Main")]
	public class Main
	{
		#region ALL_METHODS
		public static string _ToString(object obj)
		{
			if (obj is bool)
				return (bool)obj ? "true" : "false";

			if (obj.GetType().IsArray)
			{
				string ret = "{";
				Array Items = (Array)obj;

				//bool stringArray = obj.GetType().GetElementType() == typeof(string);

				foreach (object Item in Items)
				{
					bool isString = Item.GetType() == typeof(string);
					ret += (isString ? "\"" : "") + (Item != null ? Item.ToString() : "") + (isString ? "\"" : "") + ", ";
				}

				if (Items.Length > 0)
					ret = ret.Substring(0, ret.LastIndexOf(", "));

				ret += "}";
				return ret;
			}

			return obj.ToString();
		}

		[Method("Any.To" + Syntax.Types.c_String)]
		public static string ToString(Variable obj)
		{
			var result = _ToString(obj.Value);
			return result;
		}

		[Method("Any.Delete")]
		public static void Delete(Variable obj) => Interpreter.Vars.Remove(obj.Name);
		#endregion

		#region ARRAY_METHODS
		[Method(Syntax.Types.ArraySuffix + ".Length")]
		public static int ArrayLength(Variable obj) => ((Array)obj.Value).Length;

		[Method(Syntax.Types.ArraySuffix + ".Set")]
		public static void Set(Variable obj, int index, object value)
		{
			var Value = (Array)obj.Value;

			if (index < 0 || index >= Value.Length)
				throw new BaseException("Index is out of array bounds");

			Value.SetValue(value, index);
			obj.Value = Value;
		}

		[Method(Syntax.Types.ArraySuffix + ".Get")]
		public static object Get(Variable obj, int index)
		{
			var Value = ((Array)obj.Value);

			if (index < 0 || index >= Value.Length)
				throw new BaseException("Index is out of array bounds");

			return Value.GetValue(index);
		}
		#endregion

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

		[Method(Syntax.Types.c_Any + Syntax.Types.ArraySuffix + ".Create", true)]
		[Method(Syntax.Types.c_Decimal + Syntax.Types.ArraySuffix + ".Create", true)]
		[Method(Syntax.Types.c_Boolean + Syntax.Types.ArraySuffix + ".Create", true)]
		[Method(Syntax.Types.c_Character + Syntax.Types.ArraySuffix + ".Create", true)]
		[Method(Syntax.Types.c_Integer + Syntax.Types.ArraySuffix + ".Create", true)]
		[Method(Syntax.Types.c_String + Syntax.Types.ArraySuffix + ".Create", true)]
		[Method(Syntax.Types.c_Byte + Syntax.Types.ArraySuffix + ".Create", true)]
		public static Array CreateArray(Type type, int length) => Array.CreateInstance(type.rep.GetElementType(), length);

		#region STRING_METHODS
		static void VerifyParams(int index, int length = 0)
		{
			if (index < 0)
				throw new BaseException("Index cannot be less than zero");

			if (length < 0)
				throw new BaseException("Length cannot be less than zero");
		}

		[Method(Syntax.Types.String + ".Replace")]
		public static string Replace(Variable obj, string oldValue, string newValue)
		{
			var value = obj.Value.ToString().Replace(oldValue, newValue);
			return value;
		}

		[Method(Syntax.Types.String + ".OccurrencesOf")]
		public static int OccurrencesOf(Variable obj, string counter) => Utils.OccurrencesOf(obj.Value.ToString(), counter);

		[Method(Syntax.Types.String + ".GetBytes")]
		public static byte[] GetBytes(Variable obj, string encoding)
		{
			encoding = encoding.Replace("-", "");

			if (encoding == "UTF16")
				encoding = "Unicode";

			var encodingType = typeof(System.Text.Encoding).GetProperty(encoding);
			if (encodingType == null)
				throw new BaseException("Unknown encoding type: " + encoding);

			return (byte[])typeof(System.Text.Encoding)
				.GetMethod("GetBytes", new System.Type[] { typeof(string) })
				.Invoke(encodingType.GetValue(null), new object[] { (string)obj.Value });
		}

		[Method(Syntax.Types.String + ".Sub" + Syntax.Types.c_String)]
		public static string SubString(Variable obj, int index, int length)
		{
			VerifyParams(index, length);

			string str = obj.Value.ToString();

			if (index + length > str.Length)
				throw new BaseException("Index and length must refer to a location within the string");

			return str.Substring(index, length);
		}

		[Method(Syntax.Types.String + "." + Syntax.Types.c_Character + "At")]
		public static char CharacterAt(Variable obj, int index)
		{
			string str = obj.Value.ToString();

			if (index < 0 || index >= str.Length)
				throw new BaseException("Index must refer to a location within the string");

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
				throw new BaseException("Index must refer to a location within the string");

			if (index + length >= str.Length)
				throw new BaseException("Index and length must refer to a location within the string");

			return str.Remove(index, length);
		}

		[Method(Syntax.Types.String + ".Insert")]
		public static string Insert(Variable obj, int index, string value)
		{
			VerifyParams(index);

			string str = obj.Value.ToString();

			if (index >= str.Length)
				throw new BaseException("Index must refer to a location within the string");

			return str.Insert(index, value);
		}

		[Method(Syntax.Types.String + ".IndexOf")]
		public static int IndexOf(Variable obj, string value) => obj.Value.ToString().IndexOf(value);

		[Method(Syntax.Types.String + ".LastIndexOf")]
		public static int LastIndexOf(Variable obj, string value) => obj.Value.ToString().LastIndexOf(value);

		[Method(Syntax.Types.String + ".To" + Syntax.Types.c_Integer)]
		public static int StringToInteger(Variable obj)
		{
			int o;
			string str = obj.Value.ToString();

			if (Utils.IsHexadecimal(str))
				return int.Parse(str.Substring(2), NumberStyles.HexNumber);

			if (int.TryParse(str, out o))
				return o;
			else
				throw new ConversionException("Could not convert " + str + " to an " + Syntax.Types.Integer);
		}

		[Method(Syntax.Types.String + ".To" + Syntax.Types.c_Decimal)]
		public static double ToDecimal(Variable obj)
		{
			double o;
			string str = obj.Value.ToString();
			if (double.TryParse(str, out o))
				return o;
			else
				throw new ConversionException("Could not convert " + str + " to a " + Syntax.Types.Decimal);
		}
		#endregion

        #region CHARACTER_METHODS
        [Method(Syntax.Types.Character + ".ToUpper")]
		public static char ToUpper(Variable obj) => char.ToUpper((char)obj.Value);

		[Method(Syntax.Types.Character + ".ToLower")]
		public static char ToLower(Variable obj) => char.ToLower((char)obj.Value);
		#endregion

		#region CONVERSION_METHODS
		[Method(Syntax.Types.Byte + ".ToInteger")]
		[Method(Syntax.Types.Character + ".ToInteger")]
		public static int ToInteger(Variable obj) => (int)Convert.ChangeType(obj.Value, typeof(int));

		[Method(Syntax.Types.Byte + ".ToHexadecimal")]
		[Method(Syntax.Types.Integer + ".ToHexadecimal")]
		[Method(Syntax.Types.Integer64 + ".ToHexadecimal")]
		public static string ToHexadecimal(Variable obj)
		{
			object value = obj.Value;

			// must get this from reflection because it's type specific (int, long, and byte have their own version)
			var toString = value.GetType().GetMethod("ToString", new System.Type[] { typeof(string) });

			if (toString == null)
				throw new ConversionException("Cannot find ToString(string) format method from type " + obj.Type.rep.Name);

			return (string)toString.Invoke(value, new object[] { "X" });
		}

		[Method(Syntax.Types.Byte + ".ToCharacter")]
		[Method(Syntax.Types.Integer + ".ToCharacter")]
		[Method(Syntax.Types.Integer64 + ".ToCharacter")]
		public static char ToCharacter(Variable obj)
		{
			try
			{
				return (char)Convert.ChangeType(obj.Value, typeof(char));
			}
			catch (InvalidCastException)
			{
				throw new ConversionException("Could not convert " + ((long)obj.Value) + " to a " + Syntax.Types.Character);
			}
		}

		[Function("From" + Syntax.Types.c_Byte + Syntax.Types.ArraySuffix)]
		public static object FromByteArray(string typeName, byte[] bytes)
		{
			var type = Type.GetType(typeName);

			GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

			object result;
			try
			{
				result = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type.rep);
			}
			catch (MissingMethodException)
			{
				handle.Free();
				throw new BaseException("Cannot convert byte array " + _ToString(bytes) + " to a " + typeName);
			}

			handle.Free();
			return result;
		}
		#endregion

		#region BOOLEAN_METHODS
		[Method(Syntax.Types.Boolean + ".Flip")]
        public static bool Flip(Variable obj) => !(bool)obj.Value;
		#endregion

        public static void Loop(List<Line> containedLines, int i, int currentLineOffset, bool isNestedInFunction, string indexVarName, int startIndex, int endIndex)
		{
			Variable iterator = Interpreter.Vars.Create(new Variable(Type.Integer, indexVarName));

			for (long p = startIndex; p < endIndex; p++)
			{
				Scope scope = new Scope();

				iterator.Value = p;

				Executor.Execute(containedLines, i + 1 + currentLineOffset, isNestedInFunction, true);

				scope.End();
			}

			Interpreter.Vars.Remove(iterator.Name); // delete the variable after loop is done
		}

		[Function("InvokeMethod")]
		public static object InvokeMethod(string typeName, string methodName, object[] parameters)
		{
			var type = System.Type.GetType(typeName);

			if (type == null)
				throw new BaseException("Cannot find C# type: '" + typeName + "'");

			var method = type.GetMethod(methodName, parameters.Select(x => x.GetType()).ToArray());

			if (method == null)
				throw new BaseException("Cannot find method '" + methodName + "' from C# type '" + typeName + "'");

			if (!method.IsStatic)
				throw new BaseException("Cannot invoke non-static method: '" + methodName + "'");

			Type.GetType(method.ReturnType); // throw an exception if the method's return type isn't a valid CowSpeak type

			return method.Invoke(null, parameters);
		}

		[Function("GetCurrentSeconds")]
		public static double GetCurrentSeconds() => DateTime.Now.Second;

		[Function("GetCurrentMilliseconds")]
		public static double GetCurrentMilliseconds() => DateTime.Now.Millisecond;

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