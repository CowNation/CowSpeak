using System;
using System.Threading;

namespace CowSpeak{
	public class Function {
		public Func<Any> FuncDef;
		public string funcName;
		public Type type;

		public bool isVoid(){
			return type == typeof(void);
		}

		public Function(string FunctionName, Func<Any> FunctionDefinition, Type type) {
			this.type = type;
			FuncDef = FunctionDefinition;
			funcName = FunctionName;
		}
	};

	public static class Functions{
		public static Any VOID_pause() {
			Console.ReadKey();
			return new Any(VarType.Int, 0);
		}

		public static Any VOID_clearMem(){
			CowSpeak.Vars.Clear();
			return new Any(VarType.Int, 0);
		}

		public static Any VOID_exit() {
			Environment.Exit(0);
			return new Any(VarType.Int, 0);
		}

		public static Any VOID_clrConsole(){
			Console.Clear();
			return new Any(VarType.Int, 0);
		}

		public static Any inputString(){
			return new Any(VarType.String, Console.ReadLine());	
		}

		public static Any inputInt(){
			string built = "";
			ConsoleKeyInfo key = new ConsoleKeyInfo();
			while (key.Key != ConsoleKey.Enter){
				Thread.Sleep(50);

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
			return new Any(VarType.Int, _out);
		}

		public static Any inputDecimal(){
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