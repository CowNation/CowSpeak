using System.Collections.Generic;
using System;

/*
* Contains constants relating to the functionality of CowSpeak
* TODOS (In no particular order):
* Parameters for static functions & replace some keywords with static functions (run, print)
* Add user-defined functions
* Add support for negative values (signed char array)
* Add if conditional (using boolean)
*/

namespace CowSpeak{
	public class CowSpeak{
		static public List< Function > staticFX = new List< Function >{
			new Function("exit()", Functions.VOID_exit, typeof(void)),
			new Function("pause()", Functions.VOID_pause, typeof(void)),
			new Function("clrConsole()", Functions.VOID_clrConsole, typeof(void)),
			new Function("clearMem()", Functions.VOID_clearMem, typeof(void)),
			new Function("inputString()", Functions.inputString, typeof(string)),
			new Function("inputInteger()", Functions.inputInt, typeof(int)),
			new Function("inputDecimal()", Functions.inputDecimal, typeof(double))
		};

		static public Function findFunction(string functionName, bool _throw = true){
			for (int i = 0; i < staticFX.Count; i++){
				if (staticFX[i].funcName == functionName)
					return staticFX[i];
			}

			if (_throw){
				FATAL_ERROR("Function '" + functionName + "' not found");
				Functions.VOID_exit();
			}
			return null;
		} // find fuction with matching name

		static public bool shouldDebug = false;

		static public int currentLine = -1;
		static public string currentFile = "";

		public static void FATAL_ERROR(string errorStr) {
			Console.WriteLine("\n(" + currentFile + ", " + currentLine + ") FATAL_ERROR: " + errorStr);
			Console.ReadKey(); // prevent immediate closure
			currentFile = "";
			currentLine = -1;
			Environment.Exit(-1);
		}

		static public List< Variable > Vars = new List< Variable >();

		public static Variable getVariable(string varName, bool _throw = true) {
			for (int i = 0; i < Vars.Count; i++) {
				if (Vars[i].Name == varName)
					return Vars[i];
			}
			if (_throw){
				FATAL_ERROR("Could not find variable: " + varName);
				Environment.Exit(-1);
			}
			return null;
		}

		public static bool isVarDefined(string varName) {
			return getVariable(varName, false) != null;
		}

		static public void Exec(string fileName, bool shouldDebug = false){
			currentFile = fileName;
			new FileLexer(new CowConfig.readConfig(fileName).GetLines(), shouldDebug);
		}
	}
}