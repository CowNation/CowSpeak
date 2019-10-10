using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace CowSpeak{
	public static class Utils {
		public static Random rand = new Random();

		public static bool isOperator(TokenType type){
			return type.ToString().IndexOf("Operator") != -1;
		}

		public static T TryParse<T>(string inValue)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, inValue);
		}

		public static List< string > GetContainedLines(List< TokenLine > Lines, int endingBracket, int i){
			List< TokenLine > _containedLines = new List< TokenLine >();
			_containedLines = Lines.GetRange(i + 1, endingBracket - (i + 1));
			List< string > containedLines = new List< string >();

			foreach (TokenLine line in _containedLines){
				string built = "";
				foreach (Token pToken in line.tokens){
					built += pToken.identifier.Replace(Environment.NewLine, @"\n").Replace(((char)0x1f).ToString(), " ").Replace(((char)0x1D).ToString(), " ") + " ";
				}
				containedLines.Add(built);
			}

			return containedLines;
		}

		public static bool isBetween(string str, int index, char start, char end) {
			bool between = false;
			int i = 0;
			foreach (char letter in str){
				if (letter == start && between == false)
					between = true;
				else if (letter == end)
					between = false;

				if (i == index)
					return between;

				i++;
			}
			return between;
		}

		public static string AddStrings(List< string > toAdd){
			// example input is "hello ", "world", "!"
			string result = "";
			foreach (string _add in toAdd){
				result += _add;
			}
			return result;
		}

		public static double Evaluate(string expr) {
			Stack<string> stack = new Stack<string>();

			string value = "";
			for (int i = 0; i < expr.Length; i++)
			{
				string s = expr.Substring(i, 1);
				char chr = s.ToCharArray()[0];

				if (!char.IsDigit(chr) && chr != '.' && chr != ((char)26) && value != "")
				{
					stack.Push(value);
					value = "";
				}

				if (s.Equals("(")) {

					string innerExp = "";
					i++; //Fetch Next Character
					int bracketCount=0;
					for (; i < expr.Length; i++)
					{
						s = expr.Substring(i, 1);

						if (s.Equals("("))
							bracketCount++;

						if (s.Equals(")"))
							if (bracketCount == 0)
								break;
							else
								bracketCount--;


						innerExp += s;
					}

					stack.Push(Evaluate(innerExp).ToString());

				}
				else if (s.Equals("+")) stack.Push(s);
				else if (s.Equals("-")) stack.Push(s);
				else if (s.Equals("*")) stack.Push(s);
				else if (s.Equals("/")) stack.Push(s);
				else if (s.Equals("%")) stack.Push(s);
				else if (s.Equals("^")) stack.Push(s);
				else if (s.Equals(")"))
				{
				}
				else if (char.IsDigit(chr) || chr == '.' || chr == ((char)26))
				{
					if (chr == ((char)26))
						s = "-";

					value += s;

					if (value.Split('.').Length > 2)
						throw new Exception("Invalid decimal.");

					if (i == (expr.Length - 1))
						stack.Push(value);

				}
				else
					throw new Exception("Invalid character.");

			}


			double result = 0;
			while (stack.Count >= 3)
			{

				double right = Convert.ToDouble(stack.Pop());
				string op = stack.Pop();
				double left = Convert.ToDouble(stack.Pop());

				if (op == "+") result = left + right;
				else if (op == "+") result = left + right;
				else if (op == "-") result = left - right;
				else if (op == "*") result = left * right;
				else if (op == "/") result = left / right;
				else if (op == "%") result = left % right;
				else if (op == "^") result = left % right;

				stack.Push(result.ToString());
			}


			return Convert.ToDouble(stack.Pop());
		}

		public static bool isIndexValid<T>(int index, List< T > container){
			return index >= 0 && index < container.Count && container.Count > 0;
		}

		public static bool IsLettersOnly(string s)
		{
			foreach (char c in s)
			{
				if (!Char.IsLetter(c) && c != '_')
					return false;
			}
			return true;
		}

		public static bool IsDigitsOnly(string str)
		{
			foreach (char c in str)
			{
				if ((c < '0' || c > '9') && c != '.' && c != '-')
					return false;
			}

			return true;
		}
	}
}