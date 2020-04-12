using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CowSpeak
{
	internal static class Utils
	{
		public static System.Random rand = new System.Random();

		public static byte[] GetBytesFromBinaryString(string binary)
		{
			var list = new List<byte>();

			for (int i = 0; i < binary.Length; i += 8)
			{
				string t = binary.Substring(i, 8);

				list.Add(System.Convert.ToByte(t, 2));
			}

			return list.ToArray();
		}

		public static string GetStrFromHexString(string hexString)
		{
			var bytes = new byte[hexString.Length / 2];
			for (var i = 0; i < bytes.Length; i++)
			{
				bytes[i] = System.Convert.ToByte(hexString.Substring(i * 2, 2), 16);
			}

			return Encoding.Unicode.GetString(bytes);
		}

		public static int GetInitialClosingParenthesis(string str)
		{
			int skips = 0;
			for (int i = 0; i < str.Length; i++)
			{
				if (str[i] == '(')
					skips++;

				if (str[i] == ')')
				{
					skips--;
					if (skips <= 0)
						return i;
				}
			}
			return -1;
		}

		public static bool IsHexadecimal(string str) => Utils.OccurrencesOf(str, "0x") == 1 && str.IndexOf("0x") == 0;

		public static int OccurrencesOf(this string str, string splitter) => str.Split(splitter).Length - 1;

		public static string[] Split(this string str, string splitter)
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

		public static bool IsOperator(TokenType type) => type.ToString().IndexOf("Operator") != -1;

		public static T TryParse<T>(string inValue)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, inValue);
		}

		public static Type GetType(string usage, bool _throw = true)
		{
			foreach (Type type in Type.GetTypes())
				if (type.Name == usage)
					return type;

			if (_throw)
				throw new Exception("Type '" + usage + "' does not exist");

			return null;
		}

		public static List< string > GetContainedLines(List< string > Lines, int endingBracket, int i) => Lines.GetRange(i + 1, endingBracket - (i + 1));

		public static List< Line > pGetContainedLines(List< Line > Lines, int endingBracket, int i) => Lines.GetRange(i + 1, endingBracket - (i + 1));

		public static List< string > GetContainedLines(List< Line > Lines, int endingBracket, int i)
		{
			List< Line > _containedLines = new List< Line >();
			_containedLines = Lines.GetRange(i + 1, endingBracket - (i + 1));
			List< string > containedLines = new List< string >();

			foreach (Line line in _containedLines)
			{
				string built = "";
				foreach (Token pToken in line)
				{
					if (pToken.type == TokenType.String)
						built += "\"";

					built += pToken.identifier.Replace(System.Environment.NewLine, @"\n").Replace(((char)0x1f).ToString(), " ").Replace(((char)0x1D).ToString(), " ") + " ";

					if (pToken.type == TokenType.String)
						built += "\"";
				}
				containedLines.Add(built);
			}

			return containedLines;
		}

		public static bool IsIndexBetween(this string str, int index, string start, string end){
			string leftOf = str.Substring(0, index);
			int starts = leftOf.Split(start).Length - 1;

			if (start != end)
			{
				int lastStart = leftOf.LastIndexOf(start);
				int lastEnd = leftOf.LastIndexOf(end);
				int ends = leftOf.Split(end).Length - 1;
				return lastStart != -1 && (lastStart > lastEnd || starts > ends);
			}
			else
				return starts % 2 != 0;
		}

		public static bool IsIndexBetween(this string str, int index, char start, char end){
			string leftOf = str.Substring(0, index);
			int starts = leftOf.Split(start).Length - 1;

			if (start != end)
			{
				int lastStart = leftOf.LastIndexOf(start);
				int lastEnd = leftOf.LastIndexOf(end);
				int ends = leftOf.Split(end).Length - 1;
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
			return System.Convert.ToBase64String(Encoding.ASCII.GetBytes(str.Replace(System.Environment.NewLine, @"\n")));
		}

		public static string FromBase64(this string str)
		{
			return Encoding.UTF8.GetString(System.Convert.FromBase64String(str)).Replace(@"\n", System.Environment.NewLine);
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

		public static string AddStrings(List< string > toAdd)
		{
			string result = "";
			foreach (string _add in toAdd)
				result += _add;
			return result;
		}

		public static bool IsIndexValid<T>(this ICollection< T > container, int index)
		{
			return index >= 0 && index < container.Count && container.Count > 0;
		}

		public static bool IsValidObjectName(string s)
		{
			// A word char is A-z, 0-9, or a _ ([a-zA-Z0-9_] in Regex)
			// This function returns whether the entire string is only made of one or more word chars
			return Regex.Match(s, "\\w+").Value == s;
		}

		public static bool IsValidFunctionName(string s)
		{
			return Regex.Match(s, "(\\w+\\.\\w+|\\w+)").Value == s;
		}

		public static bool IsNumber(string str)
		{
			// regex is really confusing to me but basically this returns if the string is a number (including negatives and decimals)
			return Regex.Match(str, "(-|\\d*?)\\d+(\\.\\d+|\\d*?)").Value == str;
		}

		public static bool IsCharable(Token token)
		{
			if (token.type == TokenType.FunctionCall)
			{
				FunctionBase func = CowSpeak.Functions.Get(token.identifier, false);
				return func != null && func.type == Type.Character;	
			}
			else if (token.type == TokenType.VariableIdentifier)
			{
				Variable _var = CowSpeak.Vars.Get(token.identifier, false);
				return _var != null && _var.Type == Type.Character;
			}
			else if (token.type == TokenType.FunctionChain)
				return FunctionChain.GetType(token.identifier) == Type.Character;
			else
				return false;
		}

		public static bool IsStringable(Token token)
		{
			if (token.type == TokenType.FunctionCall)
			{
				FunctionBase func = CowSpeak.Functions.Get(token.identifier, false);
				return func != null && func.type == Type.String;	
			}
			else if (token.type == TokenType.VariableIdentifier)
			{
				Variable _var = CowSpeak.Vars.Get(token.identifier, false);
				return _var != null && _var.Type == Type.String;
			}
			else if (token.type == TokenType.FunctionChain)
				return FunctionChain.GetType(token.identifier) == Type.String;
			else
				return false;
		}
	}
}