using System.Collections.Generic;
using System;
using System.IO;

namespace CowSpeak{
	public class CowSpeak{
		static public List< Function > staticFX = new List< Function >{
			new Function("exit", Functions.exit, VarType.Void, "exit() - Exits the program"),
			new Function("pause", Functions.pause, VarType.Void, "pause() - Pauses the program and waits for a keypress"),
			new Function("clrConsole", Functions.clrConsole, VarType.Void, "clrConsole() - Clears all text from the console"),
			new Function("clearMem", Functions.clearMem, VarType.Void, "clearMem() - Deletes all variables from memory"),
			new Function("inputString", Functions.inputString, VarType.String, "inputString() - Allows the user to input a string"),
			new Function("inputCharacter", Functions.inputCharacter, VarType.Character, "inputString() - Allows the user to input a string"),
			new Function("inputInteger", Functions.inputInt, VarType.Integer, "inputInteger() - Allows the user to input a integer"),
			new Function("inputDecimal", Functions.inputDecimal, VarType.Decimal, "inputDecimal() - Allows the user to input a decimal"),
			new Function("print", Functions.print, VarType.Void, "print(string text) - Prints 'text' to the console", 1),
			new Function("run", Functions.run, VarType.Void, "run(string fileName) - Executes a cowfile with the name 'fileName'", 1),
			new Function("randomInteger", Functions.randomInteger, VarType.Integer, "randomInteger(integer minimum, integer maximum) - Returns a random integer with a minimum of 'minimum' and a maximum of 'maximum'", 2),
			new Function("sleep", Functions.sleep, VarType.Void, "sleep(integer milliseconds) - Program waits for 'milliseconds' milliseconds", 1),
			new Function("define", Functions.define, VarType.Void, "define(string from, string to) - Replaces all occurences of 'from' with 'to' in the code", 2),
			new Function("startRestrictedScope", Functions.startRestrictedScope, VarType.Void, "startRestrictedScope() - Starts a RestrictedScope", 0),
			new Function("endRestrictedScope", Functions.endRestrictedScope, VarType.Void, "endRestrictedScope() - Starts a RestrictedScope (Any variables or definitions created in this scope will be deleted)", 0),
			new Function("Evaluate", Functions._Evaluate, VarType.Decimal, "Evaluate(string toExec) - Evaluates toExec as an expression and returns the result", 1),

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

			if (functionName.IndexOf(".") != -1){
				functionName = functionName.Substring(functionName.IndexOf(".") + 1);
				foreach (VarType type in VarType.GetTypes()){
					for (int i = 0; i < type.methods.Length; i++){
						if (functionName.IndexOf(type.methods[i].funcName) == 0)
							return type.methods[i];
					}
				}
			} // if it has a period, it's probably a method

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

		static public void Run(string fileName, bool ishouldDebug = false){
			Exec(fileName, ishouldDebug);
			Vars.Clear();
			Definitions.Clear();
		}

		static public void Exec(string fileName, bool ishouldDebug = false){
			currentFile = fileName;
			shouldDebug = ishouldDebug;

			if (!File.Exists(fileName))
				FATAL_ERROR("Cannot execute file '" + fileName + "', it doesn't exist");
			else if (fileName.IndexOf(".COWFILE") == -1)
				FATAL_ERROR("Cannot execute file '" + fileName + "', it doesn't have the .COWFILE file extension");

			// Extremely poor fix for a VarType's method return type cannot return that VarType
			VarType.String.methods = new Function[7]{
				new Function("SubString", Functions.SubString, VarType.String, "string.SubString(integer startIndex, integer length) - Retrieves a substring from this instance starting at a specified character position and has a specified length", 2, true),
				new Function("CharacterAt", Functions.CharacterAt, VarType.Character, "string.CharacterAt(integer index) - Returns a character at 'index'", 1, true),
				new Function("Length", Functions.Length, VarType.Integer, "string.Length() - Returns the number of characters", 0, true),
				new Function("Remove", Functions.Remove, VarType.String, "string.Remove(integer startIndex, integer Count) - Returns a new string in which a specified number of characters from the current string are deleted", 2, true),
				new Function("Insert", Functions.Insert, VarType.String, "string.Insert(integer startIndex, string value) - Returns a new string in which a specified string is inserted at a specified index position in this instance", 2, true),
				new Function("IndexOf", Functions.IndexOf, VarType.Integer, "string.IndexOf(string value) - Reports the zero-based index of the first occurrence of the specified string in this instance", 1, true),
				new Function("LastIndexOf", Functions.LastIndexOf, VarType.Integer, "string.LastIndexOf(string value) - Reports the zero-based index of the last occurrence of the specified string in this instance", 1, true)
			};
			VarType.Character.methods = new Function[2]{
				new Function("ToUpper", Functions.ToUpper, VarType.Character, "character.ToUpper() - Converts the value to its uppercase equivalent", 0, true),
				new Function("ToLower", Functions.ToLower, VarType.Character, "character.ToUpper(character char) - Converts the value to its lowercase equivalent", 0, true)
			};

			new Lexer(new CowConfig.readConfig(fileName).GetLines(), shouldDebug);
		}
	}
}