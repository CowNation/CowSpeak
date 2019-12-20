using System;
using System.Collections.Generic;
using CowSpeak;
using System.Diagnostics;

class MainClass {
	public static void Main(string[] args) {
		CowSpeak.CowSpeak.Run("Examples/FizzBuzz.cf");
		CowSpeak.CowSpeak.Run("Examples/convertStr.cf");
		CowSpeak.CowSpeak.Run("Examples/stringThings.cf");
		CowSpeak.CowSpeak.Run("Examples/calculator.cf");
		CowSpeak.CowSpeak.Run("Examples/stairsMaker.cf");
		CowSpeak.CowSpeak.Run("Examples/randomNumberGuessr.cf");
	}
}