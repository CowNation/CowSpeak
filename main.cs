using System.Reflection;
using System.IO;
using System.Collections.Generic;
using CowSpeak;

static class Extensions
{
	public static int OccurrencesOf(this string str, string splitter) => str.Split(splitter).Length - 1;

	public static string[] Split(this string str, string splitter)
	{
		List<string> ret = new List<string>();
		while (str.IndexOf(splitter) != -1)
		{
			ret.Add(str.Substring(0, str.IndexOf(splitter)));
			str = str.Remove(0, str.IndexOf(splitter) + splitter.Length);
		}
		ret.Add(str);
		return ret.ToArray();
	}
}

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
				System.Console.WriteLine("(" + (ex.ErrorFile != "" ? ex.ErrorFile + ", " : "") + ex.ErrorLine + ") " + ex.Message + (ex.StackTrace != "" ? "\n" + ex.StackTrace : ""));
			}
			catch (System.Reflection.TargetInvocationException ex)
			{
				System.Console.WriteLine(ex.GetBaseException().Message);
			}
		}
	}
}