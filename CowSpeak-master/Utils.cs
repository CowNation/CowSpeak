using System.Collections.Generic;
using System;

namespace CowSpeak{
	public static class Utils{
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

		public static bool isDecimal(string input){
			return input.IndexOf(".") != -1;
		}

		public static float varToType(string input){
			if (!isDecimal(input)) // find decimal point to determine type
				return Int32.Parse(input); // type is int
			else
				return float.Parse(input); // type is float
		}

		public static void FATAL_ERROR(int failedAt, string errorStr) {
			Console.WriteLine("\n(" + failedAt + ") FATAL_ERROR: " + errorStr);
			Console.ReadKey();
			Environment.Exit(-1);
		}

		public static bool IsLettersOnly(string s)
		{
			foreach (char c in s)
			{
				if (!Char.IsLetter(c))
					return false;
			}
			return true;
		}

		public static bool IsDigitsOnly(string str)
		{
			foreach (char c in str)
			{
				if ((c < '0' || c > '9') && c != '.')
					return false;
			}

			return true;
		}

		public static bool isVarDefined(List< Variable > Vars, string varName) {
			for (int i = 0; i < Vars.Count; i++) {
				if (Vars[i].Name == varName)
					return true;
			}
			return false;
		}

		public static int getVariable(int lineNum, List< Variable > Vars, string varName) {
			for (int i = 0; i < Vars.Count; i++) {
				if (Vars[i].Name == varName)
					return i;
			}
			FATAL_ERROR(lineNum + 1, "Could not find variable: " + varName);
			Environment.Exit(-1);
			return -1;
		} // returns index of variable cuz C# hates refs and ptrs

		public static void assignDefinedVar(List< Variable > Vars, string varName, float Val) {
			for (int i = 0; i < Vars.Count; i++) {
				if (Vars[i].Name == varName){
					Vars[i].Value = Val;
					return;
				}
			}
		}

		public static List< string > Splitstring(string str, char splitter) {
			List< string > ret = new List< string >();
			string temp = "";
			for (int i = 0; i < str.Length; i++) {
				if (str[i] == splitter && temp.Length > 0) {
					ret.Add(temp);
					temp = "";
				}
				else
					temp += str[i];
			}
			if (temp.Length > 0)
				ret.Add(temp);
			return ret;
		}
	}
}