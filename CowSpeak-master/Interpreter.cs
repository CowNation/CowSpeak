using CowSpeak.Exceptions;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CowSpeak
{
	public static class Interpreter
	{
		internal static Functions Functions = new Functions();

		internal static Dictionary<string, Definition> Definitions = new Dictionary<string, Definition>();

		internal static void ClearUserDefinitions()
		{
			foreach (var pair in Definitions)
				if (pair.Value.DefinitionType == DefinitionType.User)
					Definitions.Remove(pair.Key);
		}

		public static bool Debug = false;

		public static int CurrentLine = -1;
		public static string CurrentFile = "";
		public static List< string > StackTrace = new List< string >();

		internal static Variables Vars = new Variables();
		internal static Structures Structs = new Structures();

		public static ModuleSystem ModuleSystem = new ModuleSystem();

		public static void Execute(string fileName)
		{
			CurrentFile = fileName;

			if (!File.Exists(fileName))
				throw new BaseException("Cannot execute COWFILE '" + fileName + "', it doesn't exist");

			if (!fileName.EndsWith(".cf"))
				throw new BaseException("Cannot execute file '" + fileName + "', it doesn't the cowfile extension (.cf)");

			Executor.Execute(Lexer.Parse(new CowConfig.ReadConfig(fileName).GetLines(), 0, false, false));
		}

		public static void Execute(string[] lines)
		{
			CurrentFile = "";

			Executor.Execute(Lexer.Parse(lines.ToList()));
		}

		public static void Run(string fileName)
		{
			Execute(fileName);
			Vars.Clear();
			ClearUserDefinitions();
			Functions.ClearUserDefined();
		}

		public static void Run(string[] lines)
		{
			Execute(lines);
			Vars.Clear();
			ClearUserDefinitions();
			Functions.ClearUserDefined();
		}
	}
}