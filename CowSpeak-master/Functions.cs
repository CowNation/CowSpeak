using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CowSpeak{
	public static class Functions{
		#region ALL_METHODS
		[FunctionAttr("Any.ToString", Syntax.String, "Any.ToString() - Returns a string that represents the object of any type", 0, true)]
		public static Any ToString(Any[] parameters){
			return new Any(VarType.String, parameters[0].Get().ToString());
		}
		#endregion

		#region STRING_METHODS
		[FunctionAttr(Syntax.String + ".Sub" + Syntax.c_String, Syntax.String, Syntax.String + ".Sub" + Syntax.String + "(" + Syntax.Integer + " startIndex, " + Syntax.Integer + " endIndex) - Retrieves a substring beginning at the specified 'startIndex' and extends to the character at index 'endIndex' – 1", 2, true)]
		public static Any SubString(Any[] parameters){
			string str = parameters[0].Get().ToString();
			int startIndex = (int)parameters[1].Get();
			int endIndex = (int)parameters[2].Get();

			return new Any(VarType.String, str.Substring(startIndex, endIndex - startIndex));
		}

		[FunctionAttr(Syntax.String + "." + Syntax.c_Character + "At", Syntax.Character, Syntax.String + "." + Syntax.c_Character + "At(" + Syntax.Integer + " index) - Returns a " + Syntax.Character + " at 'index'", 1, true)]
		public static Any CharacterAt(Any[] parameters){
			return new Any(VarType.Character, parameters[0].Get().ToString()[(int)parameters[1].Get()]);
		}

		[FunctionAttr(Syntax.String + ".Length", Syntax.Integer, Syntax.String + ".Length() - Returns the number of characters", 0, true)]
		public static Any Length(Any[] parameters){
			return new Any(VarType.Integer, parameters[0].Get().ToString().Length);
		}

		[FunctionAttr(Syntax.String + ".Remove", Syntax.String, Syntax.String + ".Remove(" + Syntax.Integer + " startIndex, " + Syntax.Integer + " endIndex) - Returns a new string in which all of the characters between 'startIndex' and 'endIndex' are removed", 2, true)]
		public static Any Remove(Any[] parameters){
			int startIndex = (int)parameters[1].Get();
			int endIndex = (int)parameters[2].Get();
			return new Any(VarType.String, parameters[0].Get().ToString().Remove(startIndex, endIndex - startIndex));
		}

		[FunctionAttr(Syntax.String + ".Insert", Syntax.String, Syntax.String + ".Insert(" + Syntax.Integer + " startIndex, " + Syntax.String + " value) - Returns a new " + Syntax.String + " in which a specified " + Syntax.String + " is inserted at a specified index position in this instance", 2, true)]
		public static Any Insert(Any[] parameters){
			int index = (int)parameters[1].Get();
			string toPut = parameters[2].Get().ToString();
			return new Any(VarType.String, parameters[0].Get().ToString().Insert(index, toPut));
		}

		[FunctionAttr(Syntax.String + ".IndexOf", Syntax.Integer, Syntax.String + ".IndexOf(" + Syntax.String + " value) - Reports the zero-based index of the first occurrence of the specified " + Syntax.String + " in this instance", 1, true)]
		public static Any IndexOf(Any[] parameters){
			string toSearchFor = parameters[1].Get().ToString();
			return new Any(VarType.String, parameters[0].Get().ToString().IndexOf(toSearchFor));
		}

		[FunctionAttr(Syntax.String + ".LastIndexOf", Syntax.Integer, Syntax.String + ".LastIndexOf(" + Syntax.String + " value) - Reports the zero-based index of the last occurrence of the specified " + Syntax.String + " in this instance", 1, true)]
		public static Any LastIndexOf(Any[] parameters){
			string toSearchFor = parameters[1].Get().ToString();
			return new Any(VarType.String, parameters[0].Get().ToString().LastIndexOf(toSearchFor));
		}

		[FunctionAttr(Syntax.String + ".To" + Syntax.c_Integer, Syntax.Integer, Syntax.String + ".To" + Syntax.c_Integer + "() - Returns the " + Syntax.String + " converted to an " + Syntax.Integer + ", if not possible, an error is thrown", 0, true)]
		public static Any ToInteger(Any[] parameters){
			int o;
			string str = parameters[0].Get().ToString();
			if (Int32.TryParse(str, out o))
				return new Any(VarType.Integer, o);
			else{
				CowSpeak.FATAL_ERROR("Could not convert " + Syntax.String + " to an " + Syntax.Integer);
				return null;
			}
		}

		[FunctionAttr(Syntax.String + ".To" + Syntax.c_Decimal, Syntax.Decimal, Syntax.String + ".To" + Syntax.c_Decimal + "() - Returns the " + Syntax.String + " converted to a " + Syntax.Decimal + ", if not possible, an error is thrown", 0, true)]
		public static Any ToDecimal(Any[] parameters){
			double o;
			string str = parameters[0].Get().ToString();
			if (Double.TryParse(str, out o))
				return new Any(VarType.Decimal, o);
			else{
				CowSpeak.FATAL_ERROR("Could not convert " + Syntax.String + " to an " + Syntax.Decimal);
				return null;
			}
		}
		#endregion

		#region CHARACTER_METHODS
		[FunctionAttr(Syntax.Character + ".ToUpper", Syntax.Character, Syntax.Character + ".ToUpper() - Converts the value to it's uppercase equivalent", 0, true)]
		public static Any ToUpper(Any[] parameters){
			return new Any(VarType.Character, Char.ToUpper(parameters[0].Get().ToString()[0]));
		}
		[FunctionAttr(Syntax.Character + ".ToLower", Syntax.Character, Syntax.Character + ".ToUpper() - Converts the value to it's lowercase equivalent", 0, true)]
		public static Any ToLower(Any[] parameters){
			return new Any(VarType.Character, Char.ToLower(parameters[0].Get().ToString()[0]));
		}
		#endregion

		[FunctionAttr("abs", Syntax.Decimal, "abs(" + Syntax.Decimal + " val) - Returns the absolute value of 'val'", 1)]
		public static Any abs(Any[] parameters){
			return new Any(VarType.Decimal, Math.Abs((double)parameters[0].Get()));
		}

		[FunctionAttr("round", Syntax.Integer, "round(" + Syntax.Decimal + " x) - Returns the integer nearest to x", 1)]
		public static Any round(Any[] parameters){
			return new Any(VarType.Integer, Math.Round((double)parameters[0].Get()));
		}

		[FunctionAttr("sleep", Syntax.Void, "sleep(" + Syntax.Integer + " ms) - Program waits for 'ms' milliseconds", 1)]
		public static Any sleep(Any[] parameters){
			Thread.Sleep((int)parameters[0].Get());
			return new Any(VarType.Integer, 0);
		}

		[FunctionAttr("pause", Syntax.Void, "pause() - Pauses the program and waits for a keypress")]
		public static Any pause(Any[] parameters) {
			Console.ReadKey();
			return new Any(VarType.Integer, 0);
		}

		[FunctionAttr("define", Syntax.Void, "define(" + Syntax.String + " from, " + Syntax.String + " to) - Replaces all occurences of 'from' with 'to' in the code", 2)]
		public static Any define(Any[] parameters) {
			CowSpeak.Definitions.Add(new string[]{parameters[0].Get().ToString(), parameters[1].Get().ToString()});
			return new Any(VarType.Integer, 0);
		}

		[FunctionAttr("Evaluate", Syntax.Decimal, "Evaluate(" + Syntax.String + " toExec) - Evaluates toExec as an expression and returns the result", 1)]
		public static Any _Evaluate(Any[] parameters) {
			Any evaluatedValue = new Any();
			try{
				evaluatedValue.vType = VarType.Decimal;
				evaluatedValue.Set(Evaluate.EvaluateTokens(Lexer.ParseLine(parameters[0].Get().ToString())));
				if (((double)evaluatedValue.Get()).ToString().IndexOf(".") == -1)
					evaluatedValue.vType = VarType.Integer; // decimal not found, we can convert to int
			}
			catch{
				CowSpeak.FATAL_ERROR("Could not evaluate expression");
			}

			return evaluatedValue;
		}

		private static string toStr(Any toPrep){
			return Utils.FixBoolean(toPrep.Get().ToString());
		}

		[FunctionAttr("random" + Syntax.c_Integer, Syntax.Integer, "random" + Syntax.c_Integer + "(" + Syntax.Integer + " minimum, " + Syntax.Integer + " maximum) - Returns a random " + Syntax.Integer + " with a minimum of 'minimum' and a maximum of 'maximum'", 2)]
		public static Any randomInteger(Any[] parameters){
			int minimum = (int)parameters[0].Get();
			int maximum = (int)parameters[1].Get() + 1;

			if (minimum > maximum)
				CowSpeak.FATAL_ERROR("Minimum may not be greater than the maximum");

			return new Any(VarType.Integer, Utils.rand.Next(minimum, maximum));
		}

		[FunctionAttr("print", Syntax.Void, "print(" + Syntax.String + " text) - Prints 'text' to the console", 1)]
		public static Any print(Any[] parameters){
			Console.Write(parameters[0].Get().ToString());
			return new Any(VarType.Integer, 0);
		}

		[FunctionAttr("run", Syntax.Void, "run(" + Syntax.String + " fileName) - Executes a cowfile with the name 'fileName'", 1)]
		public static Any run(Any[] parameters){
			string currentFile = CowSpeak.currentFile;
			string fileName = parameters[0].Get().ToString();
			if (File.Exists(fileName))
				CowSpeak.Exec(fileName, CowSpeak.shouldDebug); // Execute file specified
			else
				CowSpeak.FATAL_ERROR(fileName + " does not exist");
			CowSpeak.currentFile = currentFile; // curr file is not set back after exec of another file
			return new Any(VarType.Integer, 0);
		}

		[FunctionAttr("exit", Syntax.Void, "exit(" + Syntax.Integer + " exitCode) - Exits the program, returning 'exitCode'", 1)]
		public static Any exit(Any[] parameters) {
			Environment.Exit((int)parameters[0].Get());
			return new Any(VarType.Integer, 0);
		}

		[FunctionAttr("FATAL_ERROR", Syntax.Void, "FATAL_ERROR(" + Syntax.String + " errorText) - throws a FATAL_ERROR with the message 'errorText'", 1)]
		public static Any FATAL_ERROR(Any[] parameters){
			CowSpeak.FATAL_ERROR(parameters[0].Get().ToString());
			return new Any(VarType.Integer, 0);	
		}

		[FunctionAttr("input" + Syntax.c_String, Syntax.String, "input" + Syntax.String + "() - Allows the user to input a " + Syntax.String)]
		public static Any inputString(Any[] parameters){
			return new Any(VarType.String, Console.ReadLine());	
		}

		[FunctionAttr("input" + Syntax.c_Integer, Syntax.Integer, "input" + Syntax.c_Integer + "() - Allows the user to input a " + Syntax.Integer)]
		public static Any inputInteger(Any[] parameters){
			string built = "";
			ConsoleKeyInfo key = new ConsoleKeyInfo();
			while (key.Key != ConsoleKey.Enter){
				Thread.Sleep(100);

				key = Console.ReadKey();
				if ((key.KeyChar >= '0' && key.KeyChar <= '9') || (built.IndexOf("-") == -1 && key.KeyChar == '-'))
					built += key.KeyChar;
				else if (key.Key == ConsoleKey.Backspace)
					built = built.Remove(built.Length - 1, 1);
				else
					Console.Write("\b \b");
			}
			int _out = -1;
			Int32.TryParse(built, out _out);
			return new Any(VarType.Integer, _out);
		}

		[FunctionAttr("input" + Syntax.c_Character, Syntax.Character, "input" + Syntax.c_Character + "() - Allows the user to input a " + Syntax.Character)]
		public static Any inputCharacter(Any[] parameters){
			return new Any(VarType.Character, Console.ReadKey().KeyChar);
		}

		[FunctionAttr("input" + Syntax.c_Decimal, Syntax.Decimal, "input" + Syntax.c_Decimal + "() - Allows the user to input a " + Syntax.Decimal)]
		public static Any inputDecimal(Any[] parameters){
			string built = "";
			ConsoleKeyInfo key = new ConsoleKeyInfo();
			while (key.Key != ConsoleKey.Enter){
				Thread.Sleep(50);

				key = Console.ReadKey();
				if ((key.KeyChar >= '0' && key.KeyChar <= '9') || (built.IndexOf(".") == -1 && key.KeyChar == '.') || (built.IndexOf("-") == -1 && key.KeyChar == '-'))
					built += key.KeyChar;
				else if (key.Key == ConsoleKey.Backspace)
					built = built.Remove(built.Length - 1, 1);
				else
					Console.Write("\b \b");
			}
			float _out = -1;
			Single.TryParse(built, out _out);
			return new Any(VarType.Decimal, _out);
		}
	}
}