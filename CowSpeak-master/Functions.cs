using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace CowSpeak{
	public static class Functions{
		public static Any sleep(params Any[] parameters){
			Thread.Sleep((int)parameters[0].Get());
			return new Any(VarType.Integer, 0);
		}
		public static Any pause(params Any[] parameters) {
			Console.ReadKey();
			return new Any(VarType.Integer, 0);
		}


		private static string toStr(Any toPrep){
			return toPrep.Get().ToString().Replace("True", "1").Replace("False", "0");
		}

		public static Any isEqual(params Any[] parameters){
			return new Any(VarType.Boolean, toStr(parameters[0]) == toStr(parameters[1]));
		}
		public static Any isNotEqual(params Any[] parameters){
			return new Any(VarType.Boolean, toStr(parameters[0]) != toStr(parameters[1]));
		}
		public static Any isLessThan(params Any[] parameters){
			string param0 = parameters[0].Get().ToString();
			string param1 = parameters[1].Get().ToString();
			return new Any(VarType.Boolean, Convert.ToDouble(param0) < Convert.ToDouble(param1));
		}
		public static Any isGreaterThan(params Any[] parameters){
			string param0 = parameters[0].Get().ToString();
			string param1 = parameters[1].Get().ToString();
			return new Any(VarType.Boolean, Convert.ToDouble(param0) > Convert.ToDouble(param1));
		}

		public static Any randomInteger(params Any[] parameters){
			int minimum = (int)parameters[0].Get();
			int maximum = (int)parameters[1].Get() + 1;

			if (minimum > maximum)
				CowSpeak.FATAL_ERROR("Minimum may not be greater than the maximum");

			return new Any(VarType.Integer, Utils.rand.Next(minimum, maximum));
		}

		public static Any print(params Any[] parameters){
			Console.Write(parameters[0].Get().ToString());
			return new Any(VarType.Integer, 0);
		}

		public static Any run(params Any[] parameters){
			string currentFile = CowSpeak.currentFile;
			string fileName = parameters[0].Get().ToString();
			if (File.Exists(fileName))
				CowSpeak.Exec(fileName, CowSpeak.shouldDebug); // Execute file specified
			else
				CowSpeak.FATAL_ERROR(fileName + " does not exist");
			CowSpeak.currentFile = currentFile; // curr file is not set back after exec of another file
			return new Any(VarType.Integer, 0);
		}

		public static Any clearMem(params Any[] parameters){
			CowSpeak.Vars.Clear();
			return new Any(VarType.Integer, 0);
		}

		public static Any exit(params Any[] parameters) {
			Environment.Exit(0);
			return new Any(VarType.Integer, 0);
		}

		public static Any clrConsole(params Any[] parameters){
			Console.Clear();
			return new Any(VarType.Integer, 0);
		}

		public static Any inputString(params Any[] parameters){
			return new Any(VarType.String, Console.ReadLine());	
		}

		public static Any geta(params Any[] parameters){
			return new Any(VarType.Integer, 150);
		}

		public static Any inputInt(params Any[] parameters){
			string built = "";
			ConsoleKeyInfo key = new ConsoleKeyInfo();
			while (key.Key != ConsoleKey.Enter){
				Thread.Sleep(100);

				key = Console.ReadKey();
				if (key.KeyChar >= '0' && key.KeyChar <= '9')
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

		public static Any inputDecimal(params Any[] parameters){
			string built = "";
			ConsoleKeyInfo key = new ConsoleKeyInfo();
			while (key.Key != ConsoleKey.Enter){
				Thread.Sleep(50);

				key = Console.ReadKey();
				if ((key.KeyChar >= '0' && key.KeyChar <= '9') || (built.IndexOf(".") == -1 && key.KeyChar == '.'))
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