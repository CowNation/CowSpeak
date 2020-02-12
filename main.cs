using System.Reflection;
using System.IO;
using System.Collections.Generic;
using CowSpeak;

class Shell
{
	public static void Main(string[] args)
	{
		System.Console.Write("Welcome to the CowSpeak(TM) shell!\nIn order to exit the shell, call the Exit() function");

		List<string> Lines = null;
		while (true)
		{
			Lines = new List<string>();
			System.Console.Write("\n<< ");
			int StartBrackets;
			int EndBrackets;
			do
			{
				StartBrackets = 0;
				EndBrackets = 0;

				Lines.Add(System.Console.ReadLine());

				foreach (string Line in Lines)
				{
					StartBrackets += Line.OccurrencesOf("{");
					EndBrackets += Line.OccurrencesOf("}");
				}
			} while (StartBrackets != EndBrackets);

			try
			{
				CowSpeak.CowSpeak.Exec(Lines.ToArray());
			}
			catch (CowSpeak.Exception ex)
			{
				System.Console.WriteLine("(" + (ex.ErrorFile != "" ? ex.ErrorFile + ", " : "") + ex.ErrorLine + ") " + ex.Message);
			}
		}
	}
}