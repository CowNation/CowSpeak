using System;

class MainClass {
	public static void Main (string[] args) {
		CowSpeak.CowSpeak cowspeak = new CowSpeak.CowSpeak(true);
		cowspeak.Exec("main.COWFILE");
	}
}
