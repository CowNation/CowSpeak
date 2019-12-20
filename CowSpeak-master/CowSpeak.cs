using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

namespace CowSpeak{
	public class CowSpeak{
		public static List< FunctionBase > Functions = FunctionAttr.GetFunctions();

		public static List< string[] > Definitions = new List< string[] >();

		public static FunctionBase GetFunction(string functionName, bool _throw = true){
			if (functionName.IndexOf(".") != -1){
				Variable obj = GetVariable(functionName.Substring(0, functionName.IndexOf(".")), false); // interpret variable name to the left of the period as a variable

				if (obj != null)
					functionName = obj.vType.Name + functionName.Substring(functionName.IndexOf("."));
			} // if it has a period, it's probably a method

			for (int i = 0; i < Functions.Count; i++){
				if (functionName.IndexOf(Functions[i].Name) == 0)
					return Functions[i];
			}

			if (functionName.IndexOf(".") != -1){
				FunctionBase found = GetFunction("Any" + functionName.Substring(functionName.IndexOf(".")), false);
				if (found != null)
					return found;
			} // try to see if it's an 'Any' method

			if (_throw){
				FatalError("Function '" + functionName + "' not found");
				return null;
			}
			return null;
		}

		public static void ClearUserFunctions(){
			for (int i = 0; i < Functions.Count; i++){
				if (Functions[i].definitionType == DefinitionType.User)
					Functions.RemoveAt(i);
			}
		}

		public static bool Debug = false;

		public static int currentLine = -1;
		public static string currentFile = "";

		public static void FatalError(string errorStr) {
			Console.WriteLine("\n(" + (currentFile.Length != 0 ? currentFile + ", " : "") + currentLine + ") Fatal Error: " + errorStr);
			Console.ReadKey(); // prevent immediate closure
			currentFile = "";
			currentLine = -1;
			Environment.Exit(-1);
		}

		public static List< Variable > Vars = new List< Variable >();

		public static void CreateVariable(Variable variable){
			if (GetVariable(variable.Name, false) != null) // already exists
				FatalError("Cannot create variable '" + variable.Name + "', a variable by that name already exists");
			
			Vars.Add(variable);
		}

		public static Variable GetVariable(string varName, bool _throw = true) {
			for (int i = 0; i < Vars.Count; i++) {
				if (Vars[i].Name == varName)
					return Vars[i];
			}
			if (_throw){
				FatalError("Could not find variable: " + varName);
				return null;
			}
			return null;
		}

		public static void Exec(string fileName, bool _Debug = false){
			currentFile = fileName;
			Debug = _Debug;

			if (!File.Exists(fileName))
				FatalError("Cannot execute COWFILE '" + fileName + "', it doesn't exist");
			else if (fileName.IndexOf(".cf") == -1)
				FatalError("Cannot execute COWFILE '" + fileName + "', it doesn't have the .cf file extension");

			new Lexer(new CowConfig.ReadConfig(fileName).GetLines());
		}

		public static void Exec(string[] lines, bool _Debug = false){
			currentFile = "";
			Debug = _Debug;

			new Lexer(lines.ToList());
		}

		public static void Run(string fileName, bool _Debug = false){
			Exec(fileName, _Debug);
			Vars.Clear();
			Definitions.Clear();
			ClearUserFunctions();
		}

		public static void Run(string[] lines, bool _Debug = false){
			Exec(lines, _Debug);
			Vars.Clear();
			Definitions.Clear();
			ClearUserFunctions();
		}
	}
}