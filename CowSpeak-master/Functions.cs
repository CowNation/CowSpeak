using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CowSpeak{
	public static class Functions{
		#region STRING_METHODS
		[_Function(Syntax.String + ".SubString", Syntax.String, Syntax.String + "SubString(integer startIndex, integer length) - Retrieves a substring from this instance starting at a specified character position and has a specified length", 2, true)]
		public static Any SubString(Any[] parameters){
			string str = parameters[0].Get().ToString();
			int index = (int)parameters[1].Get();
			int length = (int)parameters[2].Get();

			return new Any(VarType.String, str.Substring(index, length));
		}

		[_Function(Syntax.String + ".CharacterAt", Syntax.Character, Syntax.String + "CharacterAt(integer index) - Returns a character at 'index'", 1, true)]
		public static Any CharacterAt(Any[] parameters){
			return new Any(VarType.Character, parameters[0].Get().ToString()[(int)parameters[1].Get()]);
		}

		[_Function(Syntax.String + ".Length", Syntax.Integer, Syntax.String + "Length() - Returns the number of characters", 0, true)]
		public static Any Length(Any[] parameters){
			return new Any(VarType.Integer, parameters[0].Get().ToString().Length);
		}

		[_Function(Syntax.String + ".Remove", Syntax.String, Syntax.String + "Remove(integer startIndex, integer Count) - Returns a new string in which a specified number of characters from the current string are deleted", 2, true)]
		public static Any Remove(Any[] parameters){
			int index = (int)parameters[1].Get();
			int length = (int)parameters[2].Get();
			return new Any(VarType.String, parameters[0].Get().ToString().Remove(index, length));
		}

		[_Function(Syntax.String + ".Insert", Syntax.String, Syntax.String + "Insert(integer startIndex, string value) - Returns a new string in which a specified string is inserted at a specified index position in this instance", 2, true)]
		public static Any Insert(Any[] parameters){
			int index = (int)parameters[1].Get();
			string toPut = parameters[2].Get().ToString();
			return new Any(VarType.String, parameters[0].Get().ToString().Insert(index, toPut));
		}

		[_Function(Syntax.String + ".IndexOf", Syntax.Integer, Syntax.String + "IndexOf(string value) - Reports the zero-based index of the first occurrence of the specified string in this instance", 1, true)]
		public static Any IndexOf(Any[] parameters){
			string toSearchFor = parameters[1].Get().ToString();
			return new Any(VarType.String, parameters[0].Get().ToString().IndexOf(toSearchFor));
		}

		[_Function(Syntax.String + ".LastIndexOf", Syntax.Integer, Syntax.String + "LastIndexOf(string value) - Reports the zero-based index of the last occurrence of the specified string in this instance", 1, true)]
		public static Any LastIndexOf(Any[] parameters){
			string toSearchFor = parameters[1].Get().ToString();
			return new Any(VarType.String, parameters[0].Get().ToString().LastIndexOf(toSearchFor));
		}
		#endregion

		#region CHARACTER_METHODS
		[_Function(Syntax.Character + ".ToUpper", Syntax.Character, "character.ToUpper() - Converts the value to its uppercase equivalent", 0, true)]
		public static Any ToUpper(Any[] parameters){
			return new Any(VarType.Character, Char.ToUpper(parameters[0].Get().ToString()[0]));
		}
		[_Function(Syntax.Character + ".ToLower", Syntax.Character, "character.ToUpper(character char) - Converts the value to its lowercase equivalent", 0, true)]
		public static Any ToLower(Any[] parameters){
			return new Any(VarType.Character, Char.ToLower(parameters[0].Get().ToString()[0]));
		}
		#endregion

		[_Function("sleep", Syntax.Void, "sleep(integer milliseconds) - Program waits for 'milliseconds' milliseconds", 1)]
		public static Any sleep(Any[] parameters){
			Thread.Sleep((int)parameters[0].Get());
			return new Any(VarType.Integer, 0);
		}

		[_Function("pause", Syntax.Void, "pause() - Pauses the program and waits for a keypress")]
		public static Any pause(Any[] parameters) {
			Console.ReadKey();
			return new Any(VarType.Integer, 0);
		}

		private static RestrictedScope rs = null;

		[_Function("startRestrictedScope", Syntax.Void, "startRestrictedScope() - Starts a RestrictedScope", 0)]
		public static Any startRestrictedScope(Any[] parameters){
			if (rs != null)
				CowSpeak.FATAL_ERROR("RestrictedScope is already active. End it first before starting it again by using endRestrictedScope()");

			rs = new RestrictedScope();
			return new Any(VarType.Integer, 0);
		}

		[_Function("endRestrictedScope", Syntax.Void, "endRestrictedScope() - Ends a RestrictedScope (Any variables or definitions created in this scope will be deleted)", 0)]
		public static Any endRestrictedScope(Any[] parameters){
			if (rs == null)
				CowSpeak.FATAL_ERROR("RestrictedScope is not started, activate start it first with startRestrictedScope()");

			rs.End();
			rs = null;
			return new Any(VarType.Integer, 0);
		}

		[_Function("define", Syntax.Void, "define(string from, string to) - Replaces all occurences of 'from' with 'to' in the code", 2)]
		public static Any define(Any[] parameters) {
			CowSpeak.Definitions.Add(new string[]{parameters[0].Get().ToString(), parameters[1].Get().ToString()});
			return new Any(VarType.Integer, 0);
		}

		[_Function("Evaluate", Syntax.Decimal, "Evaluate(string toExec) - Evaluates toExec as an expression and returns the result", 1)]
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

		[_Function("randomInteger", Syntax.Integer, "randomInteger(integer minimum, integer maximum) - Returns a random integer with a minimum of 'minimum' and a maximum of 'maximum'", 2)]
		public static Any randomInteger(Any[] parameters){
			int minimum = (int)parameters[0].Get();
			int maximum = (int)parameters[1].Get() + 1;

			if (minimum > maximum)
				CowSpeak.FATAL_ERROR("Minimum may not be greater than the maximum");

			return new Any(VarType.Integer, Utils.rand.Next(minimum, maximum));
		}

		[_Function("print", Syntax.Void, "print(string text) - Prints 'text' to the console", 1)]
		public static Any print(Any[] parameters){
			Console.Write(parameters[0].Get().ToString());
			return new Any(VarType.Integer, 0);
		}

		[_Function("run", Syntax.Void, "run(string fileName) - Executes a cowfile with the name 'fileName'", 1)]
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

		[_Function("clearMem", Syntax.Void, "clearMem() - Deletes all variables from memory")]
		public static Any clearMem(Any[] parameters){
			CowSpeak.Vars.Clear();
			return new Any(VarType.Integer, 0);
		}

		[_Function("exit", Syntax.Void, "exit(integer exitCode) - Exits the program, returning 'exitCode'", 1)]
		public static Any exit(Any[] parameters) {
			Environment.Exit((int)parameters[0].Get());
			return new Any(VarType.Integer, 0);
		}

		[_Function("clrConsole", Syntax.Void, "clrConsole() - Clears all text from the console")]
		public static Any clrConsole(Any[] parameters){
			Console.Clear();
			return new Any(VarType.Integer, 0);
		}

		[_Function("inputString", Syntax.String, "inputString() - Allows the user to input a string")]
		public static Any inputString(Any[] parameters){
			return new Any(VarType.String, Console.ReadLine());	
		}

		[_Function("inputInteger", Syntax.Integer, "inputInteger() - Allows the user to input a integer")]
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

		[_Function("inputCharacter", Syntax.Character, "inputCharacter() - Allows the user to input a character")]
		public static Any inputCharacter(Any[] parameters){
			return new Any(VarType.Character, Console.ReadKey().KeyChar);
		}

		[_Function("inputDecimal", Syntax.Decimal, "inputDecimal() - Allows the user to input a decimal")]
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