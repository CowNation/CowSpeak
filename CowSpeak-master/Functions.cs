using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CowSpeak{
	public static class Functions{
		#region ALL_METHODS
		[FunctionAttr("Any.ToString", Syntax.String, "", true)]
		public static Any ToString(Any[] parameters){
			return new Any(Type.String, parameters[0].Get().ToString());
		}
		#endregion

		#region STRING_METHODS
		[FunctionAttr(Syntax.String + ".Sub" + Syntax.c_String, Syntax.String, Syntax.Integer + " startIndex, " + Syntax.Integer + " endIndex", true)]
		public static Any SubString(Any[] parameters){
			string str = parameters[0].Get().ToString();
			int startIndex = (int)parameters[1].Get();
			int endIndex = (int)parameters[2].Get();

			return new Any(Type.String, str.Substring(startIndex, endIndex - startIndex));
		}

		[FunctionAttr(Syntax.String + "." + Syntax.c_Character + "At", Syntax.Character, Syntax.Integer + " index", true)]
		public static Any CharacterAt(Any[] parameters){
			return new Any(Type.Character, parameters[0].Get().ToString()[(int)parameters[1].Get()]);
		}

		[FunctionAttr(Syntax.String + ".Length", Syntax.Integer, "", true)]
		public static Any Length(Any[] parameters){
			return new Any(Type.Integer, parameters[0].Get().ToString().Length);
		}

		[FunctionAttr(Syntax.String + ".Remove", Syntax.String, Syntax.Integer + " startIndex, " + Syntax.Integer + " endIndex", true)]
		public static Any Remove(Any[] parameters){
			int startIndex = (int)parameters[1].Get();
			int endIndex = (int)parameters[2].Get();
			return new Any(Type.String, parameters[0].Get().ToString().Remove(startIndex, endIndex - startIndex));
		}

		[FunctionAttr(Syntax.String + ".Insert", Syntax.String, Syntax.Integer + " startIndex, " + Syntax.String + " value", true)]
		public static Any Insert(Any[] parameters){
			int index = (int)parameters[1].Get();
			string toPut = parameters[2].Get().ToString();
			return new Any(Type.String, parameters[0].Get().ToString().Insert(index, toPut));
		}

		[FunctionAttr(Syntax.String + ".IndexOf", Syntax.Integer, Syntax.String + " value", true)]
		public static Any IndexOf(Any[] parameters){
			string toSearchFor = parameters[1].Get().ToString();
			return new Any(Type.String, parameters[0].Get().ToString().IndexOf(toSearchFor));
		}

		[FunctionAttr(Syntax.String + ".LastIndexOf", Syntax.Integer, Syntax.String + " value", true)]
		public static Any LastIndexOf(Any[] parameters){
			string toSearchFor = parameters[1].Get().ToString();
			return new Any(Type.String, parameters[0].Get().ToString().LastIndexOf(toSearchFor));
		}

		[FunctionAttr(Syntax.String + ".To" + Syntax.c_Integer, Syntax.Integer, "", true)]
		public static Any ToInteger(Any[] parameters){
			int o;
			string str = parameters[0].Get().ToString();
			if (System.Int32.TryParse(str, out o))
				return new Any(Type.Integer, o);
			else{
				CowSpeak.FatalError("Could not convert " + Syntax.String + " to an " + Syntax.Integer);
				return null;
			}
		}

		[FunctionAttr(Syntax.String + ".To" + Syntax.c_Decimal, Syntax.Decimal, "", true)]
		public static Any ToDecimal(Any[] parameters){
			double o;
			string str = parameters[0].Get().ToString();
			if (System.Double.TryParse(str, out o))
				return new Any(Type.Decimal, o);
			else{
				CowSpeak.FatalError("Could not convert " + Syntax.String + " to an " + Syntax.Decimal);
				return null;
			}
		}
		#endregion

		#region CHARACTER_METHODS
		[FunctionAttr(Syntax.Character + ".ToUpper", Syntax.Character, "", true)]
		public static Any ToUpper(Any[] parameters){
			return new Any(Type.Character, System.Char.ToUpper(parameters[0].Get().ToString()[0]));
		}
		[FunctionAttr(Syntax.Character + ".ToLower", Syntax.Character, "", true)]
		public static Any ToLower(Any[] parameters){
			return new Any(Type.Character, System.Char.ToLower(parameters[0].Get().ToString()[0]));
		}
		#endregion

		[FunctionAttr("abs", Syntax.Decimal, Syntax.Decimal + " value")]
		public static Any abs(Any[] parameters){
			return new Any(Type.Decimal, System.Math.Abs((double)parameters[0].Get()));
		}

		[FunctionAttr("round", Syntax.Integer, Syntax.Decimal + " value")]
		public static Any round(Any[] parameters){
			return new Any(Type.Integer, System.Math.Round((double)parameters[0].Get()));
		}

		[FunctionAttr("sleep", Syntax.Void, Syntax.Integer + " ms")]
		public static Any sleep(Any[] parameters){
			Thread.Sleep((int)parameters[0].Get());
			return new Any(Type.Integer, 0);
		}

		[FunctionAttr("pause", Syntax.Void, "")]
		public static Any pause(Any[] parameters) {
			System.Console.ReadKey();
			return new Any(Type.Integer, 0);
		}

		[FunctionAttr("define", Syntax.Void, Syntax.String + " from, " + Syntax.String + " to")]
		public static Any define(Any[] parameters) {
			CowSpeak.Definitions.Add(new string[]{parameters[0].Get().ToString(), parameters[1].Get().ToString()});
			return new Any(Type.Integer, 0);
		}

		[FunctionAttr("Evaluate", Syntax.Decimal, Syntax.String + " toExec")]
		public static Any _Evaluate(Any[] parameters) {
			Any evaluatedValue = new Any();
			try{
				evaluatedValue.vType = Type.Decimal;
				evaluatedValue.Set(Evaluate.EvaluateTokens(Lexer.ParseLine(parameters[0].Get().ToString())));
				if (((double)evaluatedValue.Get()).ToString().IndexOf(".") == -1)
					evaluatedValue.vType = Type.Integer; // decimal not found, we can convert to int
			}
			catch{
				CowSpeak.FatalError("Could not evaluate expression");
			}

			return evaluatedValue;
		}

		private static string toStr(Any toPrep){
			return Utils.FixBoolean(toPrep.Get().ToString());
		}

		[FunctionAttr("random" + Syntax.c_Integer, Syntax.Integer, Syntax.Integer + " minimum, " + Syntax.Integer + " maximum")]
		public static Any randomInteger(Any[] parameters){
			int minimum = (int)parameters[0].Get();
			int maximum = (int)parameters[1].Get() + 1;

			if (minimum > maximum)
				CowSpeak.FatalError("Minimum may not be greater than the maximum");

			return new Any(Type.Integer, Utils.rand.Next(minimum, maximum));
		}

		[FunctionAttr("print", Syntax.Void, Syntax.Any + " text")]
		public static Any print(Any[] parameters){
			System.Console.Write(parameters[0].Get().ToString());
			return new Any(Type.Integer, 0);
		}

		[FunctionAttr("run", Syntax.Void, Syntax.String + " fileName")]
		public static Any run(Any[] parameters){
			string currentFile = CowSpeak.currentFile;
			string fileName = parameters[0].Get().ToString();
			if (File.Exists(fileName))
				CowSpeak.Exec(fileName, CowSpeak.shouldDebug); // Execute file specified
			else
				CowSpeak.FatalError(fileName + " does not exist");
			CowSpeak.currentFile = currentFile; // curr file is not set back after exec of another file
			return new Any(Type.Integer, 0);
		}

		[FunctionAttr("exit", Syntax.Void, Syntax.Integer + " exitCode")]
		public static Any exit(Any[] parameters) {
			System.Environment.Exit((int)parameters[0].Get());
			return new Any(Type.Integer, 0);
		}

		[FunctionAttr("ThrowError", Syntax.Void, Syntax.String + " errorText")]
		public static Any ThrowError(Any[] parameters){
			CowSpeak.FatalError(parameters[0].Get().ToString());
			return new Any(Type.Integer, 0);	
		}

		[FunctionAttr("input" + Syntax.c_String, Syntax.String, "")]
		public static Any inputString(Any[] parameters){
			return new Any(Type.String, System.Console.ReadLine());	
		}

		[FunctionAttr("input" + Syntax.c_Integer, Syntax.Integer, "")]
		public static Any inputInteger(Any[] parameters){
			string built = "";
			System.ConsoleKeyInfo key = new System.ConsoleKeyInfo();
			while (key.Key != System.ConsoleKey.Enter){
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
			return new Any(Type.Integer, _out);
		}

		[FunctionAttr("input" + Syntax.c_Character, Syntax.Character, "")]
		public static Any inputCharacter(Any[] parameters){
			return new Any(Type.Character, System.Console.ReadKey().KeyChar);
		}

		[FunctionAttr("input" + Syntax.c_Decimal, Syntax.Decimal, "")]
		public static Any inputDecimal(Any[] parameters){
			string built = "";
			System.ConsoleKeyInfo key = new System.ConsoleKeyInfo();
			while (key.Key != System.ConsoleKey.Enter){
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
			return new Any(Type.Decimal, _out);
		}
	}
}