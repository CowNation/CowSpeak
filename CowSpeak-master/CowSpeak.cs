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

		public static void Exec(string fileName, bool _Debug = false)
		{
			if (Functions == null)
				Functions = FunctionAttr.GetFunctions();

			CurrentFile = fileName;
			Debug = _Debug;
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
				throw new Exception("Cannot execute COWFILE '" + fileName + "', it doesn't have the .bcf file extension");

			new Lexer(new CowConfig.ReadConfig(fileName).GetLines(), 0, false, false, Type);
		}

		public static void Exec(string[] lines, bool _Debug = false)
		{
			if (Functions == null)
				Functions = FunctionAttr.GetFunctions();

			CurrentFile = "";
			Debug = _Debug;

			new Lexer(lines.ToList());
		}

		public static void Run(string fileName, bool _Debug = false)
		{
			Exec(fileName, _Debug);
			Vars.Clear();
			ClearUserDefinitions();
			Functions.ClearUserDefined();
		}

		public static void Run(string[] lines, bool _Debug = false)
		{
			Exec(lines, _Debug);
			Vars.Clear();
			ClearUserDefinitions();
			Functions.ClearUserDefined();
		}
	}
}