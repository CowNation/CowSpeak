using System.Collections.Generic;
using System;
using System.IO;

namespace CowSpeak{
	public class CowSpeak{
		public static List< Function > staticFX = _Function.GetFunctions();

		public static List< string[] > Definitions = new List< string[] >();

		public static Function findFunction(string functionName, bool _throw = true){
			if (functionName.IndexOf(".") != -1){
				Variable obj = getVariable(functionName.Substring(0, functionName.IndexOf(".")), false); // interpret variable name to the left of the period as a variable

				if (obj != null)
					functionName = obj.vType.Name + functionName.Substring(functionName.IndexOf("."));
			} // if it has a period, it's probably a method

			for (int i = 0; i < staticFX.Count; i++){
				if (functionName.IndexOf(staticFX[i].Name) == 0)
					return staticFX[i];
			}

			if (_throw){
				FATAL_ERROR("Function '" + functionName + "' not found");
				return null;
			}
			return null;
		} // find fuction with matching name

		public static bool shouldDebug = false;

		public static int currentLine = -1;
		public static string currentFile = "";

		public static void FATAL_ERROR(string errorStr) {
			Console.WriteLine("\n(" + currentFile + ", " + currentLine + ") FATAL_ERROR: " + errorStr);
			Console.ReadKey(); // prevent immediate closure
			currentFile = "";
			currentLine = -1;
			Environment.Exit(-1);
		}

		public static List< Variable > Vars = new List< Variable >();

		public static Variable getVariable(string varName, bool _throw = true) {
			for (int i = 0; i < Vars.Count; i++) {
				if (Vars[i].Name == varName)
					return Vars[i];
			}
			if (_throw){
				FATAL_ERROR("Could not find variable: " + varName);
				return null;
			}
			return null;
		}

		public static bool isVarDefined(string varName) {
			return getVariable(varName, false) != null;
		}

		public static void Run(string fileName, bool ishouldDebug = false){
			Exec(fileName, ishouldDebug);
			Vars.Clear();
			Definitions.Clear();
		}

		public static void Exec(string fileName, bool ishouldDebug = false){
			currentFile = fileName;
			shouldDebug = ishouldDebug;

			if (!File.Exists(fileName))
				FATAL_ERROR("Cannot execute COWFILE '" + fileName + "', it doesn't exist");
			else if (fileName.IndexOf(".cf") == -1)
				FATAL_ERROR("Cannot execute COWFILE '" + fileName + "', it doesn't have the .cf file extension");

			new Lexer(new CowConfig.readConfig(fileName).GetLines(), shouldDebug);
		}
	}
}