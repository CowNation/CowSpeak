using CowSpeak.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CowSpeak
{
	internal static class FunctionChain
	{
		private static Regex isChain = new Regex(@"(\w+(\(.*\)\.|\.\w+(\(.*\)\.)))+(\w+\(.*\))", RegexOptions.Compiled);

		public static bool IsChain(string token)
		{
			var match = isChain.Match(token); // we check if the entire input is a match instead of using IsMatch which simply checks if there is a match
			return match.Success && match.Index == 0 && match.Length == token.Length && Utils.GetInitialClosingParenthesis(token) != token.Length - 1;
		}

		public static Any Evaluate(string identifier)
		{
			List< Any > evaluatedValues = new List< Any >();
			int Funcs = identifier.OccurrencesOf("(");
			for (int i = 0; i < Funcs; i++)
			{
				int End = identifier.IndexOf(")", i);
				string funcIdentifier = identifier.Substring(0, End + 1);

				FunctionBase chainFunc = Interpreter.Functions[funcIdentifier];
				evaluatedValues.Add(chainFunc.Execute(funcIdentifier));

				Interpreter.Vars.Create(new Variable(evaluatedValues.Last().Type, "temp_" + i, evaluatedValues.Last().Value));
				identifier = ("temp_" + i) + identifier.Substring(End + 1);

				if (Interpreter.Vars.ContainsKey("temp_" + (i - 1)))
					Interpreter.Vars.Remove("temp_" + (i - 1)); // remove last temp var
			}

			if (Interpreter.Vars.ContainsKey("temp_" + (Funcs - 1)))
				Interpreter.Vars.Remove("temp_" + (Funcs - 1));
				
			return evaluatedValues.Last();
		}

		public static Type GetType(string identifier)
		{
			Type lastType = null;
			while (identifier != "")
			{
				int closingParenthesis = Utils.GetInitialClosingParenthesis(identifier);

				string funcIdentifier = identifier.Substring(0, closingParenthesis + 1);

				if (Interpreter.Functions.FunctionExists(funcIdentifier))
					lastType = Interpreter.Functions[funcIdentifier].ReturnType;

				if (closingParenthesis == -1 || closingParenthesis >= identifier.Length || lastType == null)
					break;

				identifier = lastType.Name + identifier.Substring(closingParenthesis + 1);
				continue;
			}

			return lastType;
		}
	}
}