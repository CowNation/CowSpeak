using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CowSpeak
{
	public class CowSpeak
	{
		public static List< FunctionBase > Functions = FunctionAttr.GetFunctions();

		public static List< Definition > Definitions = new List< Definition >();

		public static FunctionBase GetFunction(string functionName, bool _throw = true)
		{
			if (functionName.IndexOf(".") != -1)
			{
				Variable obj = GetVariable(functionName.Substring(0, functionName.IndexOf(".")), false); // interpret variable name to the left of the period as a variable

				if (obj != null)
					functionName = obj.vType.Name + functionName.Substring(functionName.IndexOf("."));
			} // if it has a period, it's probably a method

			foreach (FunctionBase Function in Functions)
				if (functionName.IndexOf(Function.Name) == 0)
					return Function;

			if (functionName.IndexOf(".") != -1)
			{
				string anyMethod = "Any" + functionName.Substring(functionName.IndexOf("."));

				foreach (FunctionBase Function in Functions)
					if (anyMethod.IndexOf(Function.Name) == 0)
						return Function;
			} // try to see if it's an 'Any' method

			if (_throw)
				throw new Exception("Function '" + functionName + "' not found");

			return null;
		}

		public static void ClearUserFunctions()
		{
			for (int i = 0; i < Functions.Count; i++)
				if (Functions[i].DefinitionType == DefinitionType.User)
					Functions.RemoveAt(i);
		}

		public static void ClearUserDefinitions()
		{
			for (int i = 0; i < Definitions.Count; i++)
				if (Definitions[i].DefinitionType == DefinitionType.User)
					Definitions.RemoveAt(i);
		}

		public static bool Debug = false;

		public static int CurrentLine = -1;
		public static string CurrentFile = "";

		public static List< Variable > Vars = new List< Variable >();

		public static void CreateVariable(Variable variable)
		{
			if (GetVariable(variable.Name, false) != null) // already exists
				throw new Exception("Cannot create variable '" + variable.Name + "', a variable by that name already exists");
			
			Vars.Add(variable);
		}

		public static void CreateFunction(FunctionBase func)
		{
			if (GetFunction(func.Name, false) != null) // already exists
				throw new Exception("Cannot create function '" + func.Name + "', a function by that name already exists");
			
			Functions.Add(func);
		}

		public static Variable GetVariable(string varName, bool _throw = true)
		{
			foreach (Variable Var in Vars)
				if (Var.Name == varName)
					return Var;

			if (_throw)
				throw new Exception("Could not find variable: " + varName);

			return null;
		}

		public static void ImportModule() // The file is ran normally except all definitions and functions
		{

		}

		public static void Exec(string fileName, bool _Debug = false)
		{
			CurrentFile = fileName;
			Debug = _Debug;

			if (!File.Exists(fileName))
				throw new Exception("Cannot execute COWFILE '" + fileName + "', it doesn't exist");
			else if (fileName.IndexOf(".cf") == -1)
				throw new Exception("Cannot execute COWFILE '" + fileName + "', it doesn't have the .cf file extension");

			new Lexer(new CowConfig.ReadConfig(fileName).GetLines());
		}

		public static void Exec(string[] lines, bool _Debug = false)
		{
			CurrentFile = "";
			Debug = _Debug;

			new Lexer(lines.ToList());
		}

		public static void Run(string fileName, bool _Debug = false)
		{
			Exec(fileName, _Debug);
			Vars.Clear();
			ClearUserDefinitions();
			ClearUserFunctions();
		}

		public static void Run(string[] lines, bool _Debug = false)
		{
			Exec(lines, _Debug);
			Vars.Clear();
			ClearUserDefinitions();
			ClearUserFunctions();
		}
	}
}