using System.Collections.Generic;
using System;

namespace CowSpeak{
	public static class Utils {
		public static bool isOperator(TokenType type){
			return type.ToString().IndexOf("Operator") != -1;
		}

		public static bool isBetween(string str, int index, char start, char end) {
			bool between = false;
			int i = 0;
			foreach (char letter in str){
				if (letter == start)
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

				if (!char.IsDigit(chr) && chr != '.' && value != "")
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
				else if (char.IsDigit(chr) || chr == '.')
				{
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