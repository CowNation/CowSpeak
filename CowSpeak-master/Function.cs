using System;

namespace CowSpeak{
	public class Function {
		public Func<float> FuncDef;
		public string funcName;
		public Function(string FunctionName, Func<float> FunctionDefinition) {
			FuncDef = FunctionDefinition;
			funcName = FunctionName;
		}
		public bool isVoid(){
			return funcName.IndexOf("VOID__") == 0;
		}
	};

	public static class Functions{
		public static float VOID_pause() {
			Console.WriteLine("Press Any Key to Continue ");
			Console.ReadKey();
			return 0;
		}

		public static float VOID_exit() {
			Environment.Exit(0);
			return 0;
		}

		public static float VOID_clrConsole(){
			Console.Clear();
			return 0;
		}
	}
}