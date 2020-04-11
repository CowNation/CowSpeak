using System.Reflection;
using System.IO;
using System.Collections.Generic;
using CowSpeak;
using System.Collections;
using System.Linq;

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

	public static bool IsIndexBetween(this string str, int index, string start, string end){
		string leftOf = str.Substring(0, index);
		int starts = leftOf.Split(start).Length - 1;

		if (start != end)
		{
			int lastStart = leftOf.LastIndexOf(start);
			int lastEnd = leftOf.LastIndexOf(end);
			int ends = leftOf.Split(end).Length - 1;
			return lastStart != -1 && (lastStart > lastEnd || starts > ends);
		}
		else
			return starts % 2 != 0;
	}
}

class Shell
{
	static string[] RetrieveFunctions()
	{
		var assem = typeof(CowSpeak.CowSpeak).Assembly;

		var Functions = System.Activator.CreateInstance(typeof(List<>).MakeGenericType(assem.GetType("CowSpeak.FunctionBase")));
		Functions = assem.GetType("CowSpeak.FunctionAttr").GetMethod("GetFunctions").Invoke(null, new object[]{});
		var FunctionsList = (IList)Functions;
		List<string> FunctionUsages = new List<string>();
		for (int i = 0; i < FunctionsList.Count; i++)
		{
			object item = FunctionsList[i]; // type of this is CowSpeak.FunctionBase
			;
			FunctionUsages.Add((string)item.GetType().GetProperty("Usage", BindingFlags.Public | BindingFlags.Instance).GetValue(item));
		}

		FunctionUsages.Sort();

		return FunctionUsages.ToArray();
	}

	public static void Main(string[] args)
	{
		System.Console.WriteLine("Welcome to the CowSpeak(TM) shell!\nIn order to exit the shell, call the Exit() function");

		var Functions = RetrieveFunctions();
		System.Console.WriteLine("Functions (" + Functions.Length + "):");
		foreach (var Function in Functions)
			System.Console.WriteLine(Function);

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