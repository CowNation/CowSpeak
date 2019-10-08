using System.Collections.Generic;
using System;

namespace CowSpeak{
	public class CowSpeak{
		static public List< Function > staticFX = new List< Function >{
			new Function("exit", Functions.exit, VarType.Void, "exit()"),
			new Function("pause", Functions.pause, VarType.Void, "pause()"),
			new Function("clrConsole", Functions.clrConsole, VarType.Void, "clrConsole()"),
			new Function("clearMem", Functions.clearMem, VarType.Void, "clearMem()"),
			new Function("inputString", Functions.inputString, VarType.String, "inputString()"),
			new Function("inputInteger", Functions.inputInt, VarType.Int, "inputInteger()"),
			new Function("inputDecimal", Functions.inputDecimal, VarType.Decimal, "inputDecimal()"),
			new Function("print", Functions.print, VarType.Void, "print(string text)", 1),
			new Function("run", Functions.run, VarType.Void, "run(string fileName)", 1),
			new Function("randomInteger", Functions.randomInteger, VarType.Int, "randomInteger(integer minimum, integer maximum)", 2),
			new Function("sleep", Functions.sleep, VarType.Void, "sleep(integer milliseconds)", 1),

			// Comparison Functions
			new Function("isEqual", Functions.isEqual, VarType.Boolean, "isEqual(left, right)", 2),
			new Function("isNotEqual", Functions.isNotEqual, VarType.Boolean, "isNotEqual(left, right)", 2),
			new Function("isLessThan", Functions.isLessThan, VarType.Boolean, "isLessThan(left, right)", 2),
			new Function("isGreaterThan", Functions.isGreaterThan, VarType.Boolean, "isGreaterThan(left, right)", 2),
		};

		static public Function findFunction(string functionName, bool _throw = true){
			for (int i = 0; i < staticFX.Count; i++){
				if (functionName.IndexOf(staticFX[i].funcName) == 0)
					return staticFX[i];
			}

			if (_throw){
				FATAL_ERROR("Function '" + functionName + "' not found");
				Functions.exit();
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
			new Lexer(new CowConfig.readConfig(fileName).GetLines(), shouldDebug);
		}
	}
}