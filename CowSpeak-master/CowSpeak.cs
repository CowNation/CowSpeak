using System.Collections.Generic;
using System;

namespace CowSpeak{
	public class CowSpeak{
		static public List< Function > staticFX = new List< Function >{
			new Function("exit", Functions.exit, VarType.Void, "exit() - Exits the program"),
			new Function("pause", Functions.pause, VarType.Void, "pause() - Pauses the program and waits for a keypress"),
			new Function("clrConsole", Functions.clrConsole, VarType.Void, "clrConsole() - Clears all text from the console"),
			new Function("clearMem", Functions.clearMem, VarType.Void, "clearMem() - Deletes all variables from memory"),
			new Function("inputString", Functions.inputString, VarType.String, "inputString() - Allows the user to input a string"),
			new Function("inputInteger", Functions.inputInt, VarType.Integer, "inputInteger() - Allows the user to input a int"),
			new Function("inputDecimal", Functions.inputDecimal, VarType.Decimal, "inputDecimal() - Allows the user to input a decimal"),
			new Function("print", Functions.print, VarType.Void, "print(string text) - Prints 'text' to the console", 1),
			new Function("run", Functions.run, VarType.Void, "run(string fileName) - Executes a cowfile with the name 'fileName'", 1),
			new Function("randomInteger", Functions.randomInteger, VarType.Integer, "randomInteger(integer minimum, integer maximum) - Returns a random integer with a minimum of 'minimum' and a maximum of 'maximum'", 2),
			new Function("sleep", Functions.sleep, VarType.Void, "sleep(integer milliseconds) - Program waits for 'milliseconds' milliseconds", 1),
			new Function("define", Functions.define, VarType.Void, "define(string from, string to) - Replaces all occurences of 'from' with 'to' in the code", 2),
			new Function("startRestrictedScope", Functions.startRestrictedScope, VarType.Void, "startRestrictedScope() - Starts a RestrictedScope", 0),
			new Function("endRestrictedScope", Functions.endRestrictedScope, VarType.Void, "endRestrictedScope() - Starts a RestrictedScope (Any variables or definitions created in this scope will be deleted)", 0),

			// Comparison Functions
			new Function("isEqual", Functions.isEqual, VarType.Boolean, "isEqual(left, right) - Returns whether left and right are equal", 2),
			new Function("isNotEqual", Functions.isNotEqual, VarType.Boolean, "isNotEqual(left, right) - Returns whether left and right are not equal", 2),
			new Function("isLessThan", Functions.isLessThan, VarType.Boolean, "isLessThan(left, right) - Returns whether left is less than right", 2),
			new Function("isGreaterThan", Functions.isGreaterThan, VarType.Boolean, "isGreaterThan(left, right) - Returns whether left is greater than right", 2),
		};

		static public List< string[] > Definitions = new List< string[] >();

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

		static public void Exec(string fileName, bool ishouldDebug = false){
			currentFile = fileName;
			shouldDebug = ishouldDebug;

			new Lexer(new CowConfig.readConfig(fileName).GetLines(), shouldDebug);
		}
	}
}