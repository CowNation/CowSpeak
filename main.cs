using System.Collections.Generic;
using CowSpeak;
using System.Linq;
using System;
using System.Reflection;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO;

static class Extensions
{
	public static string[] Split(string str, string splitter)
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

	public static int OccurrencesOf(this string str, string splitter) => Split(str, splitter).Length - 1;
}

class Shell
{
	static void PrintFunctions()
	{
		foreach (var module in Interpreter.ModuleSystem.LoadedModules)
		{
			var functions = module.Value.DefinedFunctions;

			if (functions.Count == 0)
				continue; // prevent printing of module name if no functions are present

			Console.WriteLine("----------| " + module.Key + " |----------"); // module name
			var sortedFunctions = functions.ToList().OrderBy(function => function.Key);

			foreach (var function in sortedFunctions)
				Console.WriteLine(function.Value.Usage);
		}
	}

	public static void Main(string[] args)
	{
		PrintFunctions();

		//new CowSpeak.Module(typeof(CowSpeak.Modules.Main)).GenerateMdDocumentation("Main.md");
		//new CowSpeak.Module(typeof(CowSpeak.Modules.Windows)).GenerateMdDocumentation("Windows.md");
		//new CowSpeak.Module(typeof(CowSpeak.Modules.ShorterTypeNames)).GenerateMdDocumentation("ShorterTypeNames.md");

		Console.WriteLine("Welcome to the CowSpeak(TM) shell!\nIn order to exit the shell, call the Exit() function");

		List<string> lines = null;
		while (true)
		{
			lines = new List<string>();
			
			bool first = true;

			int startBrackets, endBrackets;
			do
			{
				startBrackets = 0;
				endBrackets = 0;

				Console.Write((first ? "\n" : "") + "<< ");
				lines.Add(Console.ReadLine());

				foreach (string line in lines)
				{
					startBrackets += line.OccurrencesOf("{");
					endBrackets += line.OccurrencesOf("}");
				}
				first = false;
			} while (startBrackets != endBrackets);

			try
			{
				Interpreter.Execute(lines.ToArray());
			}
			catch (CowSpeak.Exceptions.BaseException ex)
			{
				Console.WriteLine(ex.Message);
			}
			catch (TargetInvocationException ex)
			{
				Console.WriteLine(ex.GetBaseException().Message);
			}
		}
	}
}