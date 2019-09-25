using System;

class MainClass {
	public static void Main (string[] args) {
		CowSpeak.CowSpeak cowspeak = new CowSpeak.CowSpeak(true);
		cowspeak.Exec("main.COWFILE");

	}
}

/*
* Completely rewrote in C#
* Newlines are manual & working
* Added ability to run other COWFILES with the 'run' keyword
* Fixed print string keyword requiring no whitespace after end apostraphe
* Added failed line parameter to FATAL_ERROR for easier debugging
* Completely rewrote the system for executing code, CowSpeak class executes and holds all program memory (Vars, static functions)
* COWFILEs ran using the run keyword will share memory with other COWFILEs executed using the same CowSpeak object
*/
