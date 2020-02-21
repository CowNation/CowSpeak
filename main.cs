using System.Reflection;
using System.IO;
using System.Collections.Generic;
using CowSpeak;

class Shell
{
	public static void Main(string[] args)
	{
		System.Console.WriteLine("Welcome to the CowSpeak(TM) shell!\nIn order to exit the shell, call the Exit() function");

		List<string> Lines = null;
		while (true)
		{
			Lines = new List<string>();
			
			bool first = true;

			int StartBrackets;
			int EndBrackets;
			do
			{
				StartBrackets = 0;
				EndBrackets = 0;

				System.Console.Write((first ? "\n" : "") + "<< ");
				Lines.Add(System.Console.ReadLine());

				foreach (string Line in Lines)
				{
					StartBrackets += Line.OccurrencesOf("{");
					EndBrackets += Line.OccurrencesOf("}");
				}
				first = false;
			} while (StartBrackets != EndBrackets);

			try
			{
				CowSpeak.CowSpeak.Exec(Lines.ToArray());
			}
			catch (CowSpeak.Exception ex)
			{
				System.Console.WriteLine("(" + (ex.ErrorFile != "" ? ex.ErrorFile + ", " : "") + ex.ErrorLine + ") " + ex.Message);
			}
			catch (System.Reflection.TargetInvocationException ex)
			{
				System.Console.WriteLine(ex.GetBaseException().Message);
			}
			catch (System.Exception ex)
			{
				System.Console.WriteLine(ex.Message);
			}
		}
	}
}