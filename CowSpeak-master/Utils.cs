using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using CowSpeak.Exceptions;

namespace CowSpeak
{
	internal static class Utils
	{
		static DynamicExpresso.Interpreter interpreter = new DynamicExpresso.Interpreter();

		public static T[] ConcatArrays<T>(params T[][] p)
		{
			var position = 0;
			var outputArray = new T[p.Sum(a => a.Length)];
			foreach (var curr in p)
			{
				Array.Copy(curr, 0, outputArray, position, curr.Length);
				position += curr.Length;
			}
			return outputArray;
		}

		public static object Eval(string expression)
		{
			try
			{
				if (expression == "\'\'\'")
					return '\'';

				return interpreter.Eval(expression);
			}
			catch (Exception ex)
			{
				throw new BaseException("Couldn't evaluate expression '" + expression.Replace("\n", @"\n") + "': " + ex.Message);
			}
		}

		public static bool IsStatic(this System.Type type) => type.IsAbstract && type.IsSealed;

		public static string GetTokensExpression(List<Token> tokens, ref Any alreadyEvaluatedValue)
		{
			string expression = "";
			for (int i = 0; i < tokens.Count; i++)
			{
				string identifier = tokens[i].identifier;

				Type objectType = null;
				if (tokens[i].type == TokenType.VariableIdentifier)
				{
					Variable var = Interpreter.Vars[identifier];
					identifier = Modules.Main._ToString(var.Value); // replace variable name with it's value
					objectType = var.Type;
				}

				switch (tokens[i].type)
				{
				case TokenType.WhileConditional:
				case TokenType.IfConditional:
				case TokenType.EndBracket:
					continue;
				}

				if (tokens[i].type == TokenType.FunctionCall)
				{
					BaseFunction func = Interpreter.Functions[identifier];
					Any returned = func.Invoke(identifier);
					objectType = func.ReturnType;

					if (tokens.Count == 1)
					{
						alreadyEvaluatedValue = returned;
						return "";
					}

					if (returned != null)
						identifier = Modules.Main._ToString(returned.Value); // replace function call with it's return value
					else
						identifier = "";
				}
				else if (tokens[i].type == TokenType.FunctionChain)
				{
					Any evaluatedChain = FunctionChain.Evaluate(identifier);

					if (tokens.Count == 1)
					{
						alreadyEvaluatedValue = evaluatedChain;
						return "";
					}

					identifier = Modules.Main._ToString(evaluatedChain.Value);
					objectType = FunctionChain.GetType(tokens[i].identifier);
				}

				if (tokens[i].type == TokenType.StringLiteral || objectType == Types.String)
					identifier = "\"" + identifier.Replace("\"", "\\\"") + "\"";
				else if (tokens[i].type == TokenType.CharacterLiteral || objectType == Types.Character)
					identifier = "\'" + identifier + "\'";
				
				expression += identifier + (i < tokens.Count - 1 ? " " : "");
			}

			return expression;
		}

		public static Random rand = new Random();

		public static byte[] GetBytesFromBinaryString(string binary)
		{
			var list = new List<byte>();

			for (int i = 0; i < binary.Length; i += 8)
			{
				string t = binary.Substring(i, 8);

				list.Add(Convert.ToByte(t, 2));
			}

			return list.ToArray();
		}

		public static string GetStrFromHexString(string hexString)
		{
			var bytes = new byte[hexString.Length / 2];
			for (var i = 0; i < bytes.Length; i++)
			{
				bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);
			}

			return Encoding.Unicode.GetString(bytes);
		}

		private static Regex initialClosingParenthesis = new Regex(@"\((?>[^()]|(?<o>)\(|(?<-o>)\))*\)(?(o)(?!))", RegexOptions.Compiled);

		public static IEnumerable<MethodInfo> GetExtensionMethods(Assembly assembly, System.Type extendedType)
		{
			// pasted btw
			var query = from type in assembly.GetTypes()
						where !type.IsGenericType && !type.IsNested
						from method in type.GetMethods(BindingFlags.Static | BindingFlags.Public)
						where method.IsDefined(typeof(ExtensionAttribute), false)
						where method.GetParameters()[0].ParameterType == extendedType
						select method;
			return query;
		}

		public static int GetClosingParenthesis(string str)
		{
			Match match = initialClosingParenthesis.Match(str);
			if (match == null)
				return -1;

			return match.Index + match.Length - 1;
		}

		public static int GetClosingBracket(string str)
		{
			Match match = Regex.Match(str, @"<.+?>");
			if (match == null)
				return -1;

			return match.Index + match.Length - 1;
		}

		public static List<T> GetChildrenOfClassInstances<T>()
		{
			return typeof(T).Assembly.GetTypes().Where(t => t.IsSubclassOf(typeof(T)) && !t.IsAbstract).Select(t => (T)Activator.CreateInstance(t)).ToList();
		}

		public static bool IsHexadecimal(string str) => OccurrencesOf(str, "0x") == 1 && str.IndexOf("0x") == 0;

		public static Tuple<int, string>[] SplitWithIndicies(string str, string splitter)
		{
			List<Tuple<int, string>> ret = new List<Tuple<int, string>>();
			int Offset = 0;
			while (str.IndexOf(splitter) != -1)
			{
				ret.Add(new Tuple<int, string>(Offset, str.Substring(0, str.IndexOf(splitter))));
				Offset += splitter.Length + str.Substring(0, str.IndexOf(splitter)).Length;
				str = str.Remove(0, str.IndexOf(splitter) + splitter.Length);
			}
			ret.Add(new Tuple<int, string>(Offset, str));
			return ret.ToArray();
		}

		public static string[] Split(string str, string splitter)
		{
			List<string> ret = new List<string>();
			while (str.IndexOf(splitter) != -1)
			{
				ret.Add(str.Substring(0, str.IndexOf(splitter)));
				str = str.Remove(0, str.IndexOf(splitter) + splitter.Length);
			}
			ret.Add(str);
			return ret.ToArray();
		}

		public static string[] Split(string str, char splitter) => Split(str, splitter.ToString());

		public static int OccurrencesOf(this string str, string splitter) => Split(str, splitter).Length - 1;

		public static bool IsOperator(TokenType type) => type.ToString().IndexOf("Operator") != -1;

		public static T TryParse<T>(string inValue)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, inValue);
		}

		public static List< string > GetContainedLines(List< string > lines, int endLine, int startLine)
		{
			return lines.GetRange(startLine + 1, endLine - (startLine + 1));
		}

		public static List<Line> GetContainedLines(List< Line > lines, Executor.TokenLocation endLine, Executor.TokenLocation startLine)
		{
			if (startLine.LineIndex == endLine.LineIndex) // same line
			{
				if (startLine.TokenIndex + 1 == endLine.TokenIndex) // empty brackets
					return new List<Line>();

				return new List<Line>()
				{
					new Line(lines[startLine.LineIndex].GetRange(startLine.TokenIndex + 1, endLine.TokenIndex - (startLine.TokenIndex + 1))) // tokens between StartLine.TokenIndex & EndLine.TokenIndex
				};
			}

			return lines.GetRange(startLine.LineIndex + 1, endLine.LineIndex - (startLine.LineIndex + 1));
		}

		public static bool IsIndexBetween(this string str, int index, string start, string end){
			string leftOf = str.Substring(0, index);
			int starts = Split(leftOf, start).Length - 1;

			if (start != end)
			{
				int lastStart = leftOf.LastIndexOf(start);
				int lastEnd = leftOf.LastIndexOf(end);
				int ends = Split(leftOf, end).Length - 1;
				return lastStart != -1 && (lastStart > lastEnd || starts > ends);
			}
			else
				return starts % 2 != 0;
		}

		public static bool IsIndexBetween(this string str, int index, char start, char end){
			string leftOf = str.Substring(0, index);
			int starts = Split(leftOf, start).Length - 1;

			if (start != end)
			{
				int lastStart = leftOf.LastIndexOf(start);
				int lastEnd = leftOf.LastIndexOf(end);
				int ends = Split(leftOf, end).Length - 1;
				return lastStart != -1 && (lastStart > lastEnd || starts > ends);
			}
			else
				return starts % 2 != 0;
		}

		public static int OrdinalIndexOf(this string str, string substr, int n)
		{
			int pos = -1;
			do
			{
				pos = str.IndexOf(substr, pos + 1);
			} while (n-- > 0 && pos != -1);
			return pos;
		}

		public static string ToBase64(this string str)
		{
			return Convert.ToBase64String(Encoding.ASCII.GetBytes(str.Replace("\n", @"\n")));
		}

		public static string FromBase64(this string str)
		{
			return Encoding.UTF8.GetString(Convert.FromBase64String(str)).Replace(@"\n", "\n");
		}

		public static string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[rand.Next(s.Length)]).ToArray());
		}

		public static string ReplaceBetween(string str, char toReplace, char start, char end, char substitution = (char)0x1a){
			string _str = str;
			for (int Occurrence = 0; Occurrence < OccurrencesOf(str, toReplace.ToString()); Occurrence++)
			{
				int i = OrdinalIndexOf(str, toReplace.ToString(), Occurrence);
				char letter = str[i];

				if (letter == toReplace && str.IsIndexBetween(i, start, end))
				{
					if (substitution == (char)0x0)
						_str = _str.Remove(i, 1);
					else
					{
						StringBuilder fileLine = new StringBuilder(_str);
						fileLine[i] = substitution;
						_str = fileLine.ToString(); 
					}
				}
			}

			return _str;
		} // so you don't have to do it very inefficently with IsIndexBetween on each char

		public static bool IsIndexValid<T>(this ICollection< T > container, int index)
		{
			return index >= 0 && index < container.Count && container.Count > 0;
		}

		public static bool IsValidObjectName(string s)
		{
			// A word char is A-z, 0-9, or a _ ([a-zA-Z0-9_] in Regex)
			// This function returns whether the entire string is only made of one or more word chars
			return s.Length > 0 && Regex.Match(s, @"\w+").Value == s;
		}

		private static Regex validFunctionName = new Regex(@"(\w+\.\w+|\w+)", RegexOptions.Compiled);

		public static bool IsValidFunctionName(string s)
		{
			return s.Length > 0 && validFunctionName.Match(s).Value == s;
		}

		private static Regex isNumber = new Regex(@"(-|\d*?)\d+(\.\d+|\d*?)");

		public static bool IsNumber(string s)
		{
			return s.Length > 0 && isNumber.Match(s).Value == s;
		}
	}
}