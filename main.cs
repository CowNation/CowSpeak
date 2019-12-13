using System;
using System.Collections.Generic;
using CowSpeak;
using System.Diagnostics;

class MainClass {
	public static void Main(string[] args) {
		CowSpeak.CowSpeak.Run("convertStr.cf");
		CowSpeak.CowSpeak.Run("stringThings.cf");
		CowSpeak.CowSpeak.Run("calculator.cf");
		CowSpeak.CowSpeak.Run("stairsMaker.cf");
		CowSpeak.CowSpeak.Run("randomNumberGuessr.cf");
	}
}