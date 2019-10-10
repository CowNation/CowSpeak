using System;
using System.Collections.Generic;
using CowSpeak;

class MainClass {
	public static void Main (string[] args) {
		List< Token > Tokens = new List< Token >{
			new Token(TokenType.Number, "5"),
			new Token(TokenType.AddOperator, "+"),
			new Token(TokenType.Number, "5"),
			new Token(TokenType.MultiplyOperator, "*"),
			new Token(TokenType.Number, "2")
		};
		Console.WriteLine(Evaluate.EvaluateTokens(Tokens));
		CowSpeak.CowSpeak.Exec("main.COWFILE", false);
	}
}