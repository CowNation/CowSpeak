using System;
using System.Collections.Generic;
using CowSpeak;
using System.Diagnostics;

class MainClass {
	public static void Main(string[] args) {
		//CowSpeak.CowSpeak.Run("CowSpeak.cf", false);
		CowSpeak.CowSpeak.Run("convertStr.cf", false);
		CowSpeak.CowSpeak.Run("stringThings.cf", false);
		CowSpeak.CowSpeak.Run("calculator.cf", false);
		CowSpeak.CowSpeak.Run("stairsMaker.cf", false);
		CowSpeak.CowSpeak.Run("randomNumberGuessr.cf", false);
	}
}