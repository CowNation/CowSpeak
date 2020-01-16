using System;

class MainClass
{
	public static void Main(string[] args)
	{
		Console.Clear();

		try
		{
			CowSpeak.CowSpeak.Run("Examples/fizzBuzz.cf");
			CowSpeak.CowSpeak.Run("Examples/convertStr.cf");
			CowSpeak.CowSpeak.Run("Examples/stringThings.cf");
			CowSpeak.CowSpeak.Run("Examples/calculator.cf");
			CowSpeak.CowSpeak.Run("Examples/stairsMaker.cf");
			CowSpeak.CowSpeak.Run("Examples/randomNumberGuessr.cf");
		}
		catch (CowSpeak.Exception ex)
		{
			Console.WriteLine("(" + ex.ErrorFile + ", " + ex.ErrorLine + ") " + ex.Message);
		}
	}
}