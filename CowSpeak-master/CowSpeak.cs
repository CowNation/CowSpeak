using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CowSpeak
{
	public class CowSpeak
	{
		internal static Functions Functions = null;

		internal static List< Definition > Definitions = new List< Definition >();

		internal static void ClearUserDefinitions()
		{
			for (int i = 0; i < Definitions.Count; i++)
				if (Definitions[i].DefinitionType == DefinitionType.User)
					Definitions.RemoveAt(i);
		}

		public static bool Debug = false;

		public static int CurrentLine = -1;
		public static string CurrentFile = "";
		public static List< string > StackTrace = new List< string >();

		internal static Variables Vars = new Variables();

		public static void Execute(string fileName)
		{
			if (Functions == null)
				Functions = FunctionAttribute.GetFunctions();

			CurrentFile = fileName;

			if (!File.Exists(fileName))
				throw new Exception("Cannot execute COWFILE '" + fileName + "', it doesn't exist");

			if (fileName.IndexOf(".cf") == -1)
				throw new Exception("Cannot execute file '" + fileName + "', it doesn't the cowfile extension (.cf)");

			Executor.Execute(Lexer.Parse(new CowConfig.ReadConfig(fileName).GetLines(), 0, false, false));
		}

		public static void Execute(string[] lines)
		{
			if (Functions == null)
				Functions = FunctionAttribute.GetFunctions();

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