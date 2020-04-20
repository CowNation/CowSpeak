using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CowSpeak
{
	public enum FileType
	{
		Normal,
		Binary,
		Hex
	}

	public class CowSpeak
	{
		internal static FunctionList Functions = null;

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

		internal static VariableList Vars = new VariableList();

		public static void Execute(string fileName)
		{
			if (Functions == null)
				Functions = FunctionAttr.GetFunctions();

			CurrentFile = fileName;
			FileType Type;

			if (!File.Exists(fileName))
				throw new Exception("Cannot execute COWFILE '" + fileName + "', it doesn't exist");

			if (fileName.IndexOf(".cf") != -1)
				Type = FileType.Normal;
			else if (fileName.IndexOf(".bcf") != -1)
				Type = FileType.Binary;
			else if (fileName.IndexOf(".hcf") != -1)
				Type = FileType.Hex;
			else
				throw new Exception("Cannot execute file '" + fileName + "', it doesn't have a compatible file extension (.cf/.bcf/.hcf)");

			Executor.Execute(Lexer.Parse(new CowConfig.ReadConfig(fileName).GetLines(), 0, false, false, Type));
		}

		public static void Execute(string[] lines)
		{
			if (Functions == null)
				Functions = FunctionAttr.GetFunctions();

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