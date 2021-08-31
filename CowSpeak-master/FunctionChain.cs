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
			return match.Success && match.Index == 0 && match.Length == token.Length && Utils.GetClosingParenthesis(token) != token.Length - 1;
		}

		public static int GetChildrenCount(string identifier)
		{
			int children = 1, j = Utils.GetClosingParenthesis(identifier);
			while (j != -1 && j < identifier.Length)
			{
				children++;

				string a = identifier.Substring(j);

				j += Utils.GetClosingParenthesis(a) + 1;
			}
			return children;
		}


		public static Any Evaluate(string identifier)
		{
			List<string> temporaryParameters = new List<string>();
			List<Any> evaluatedValues = new List<Any>();

			int functions = GetChildrenCount(identifier);

			try
			{
				for (int i = 0; i < functions; i++)
				{
					int end = Utils.GetClosingParenthesis(identifier);//identifier.IndexOf(")", i);
					string funcIdentifier = identifier.Substring(0, end + 1);

					BaseFunction chainFunc = Interpreter.Functions[funcIdentifier];
					evaluatedValues.Add(chainFunc.Invoke(funcIdentifier));

					if (evaluatedValues.Last() == null)
					{
						if (i + 1 < functions)
							throw new BaseException("Cannot evaluate a FunctionChain when one if it's members returns null (excluding the last function)");
					}
					else
					{
						string temporaryName = Utils.RandomString(10);
						Interpreter.Vars.Create(new Variable(evaluatedValues.Last().Type, temporaryName, evaluatedValues.Last().Value));
						identifier = temporaryName + identifier.Substring(end + 1);

						if (temporaryParameters.Count > 0 && Interpreter.Vars.ContainsKey(temporaryParameters.Last()))
						{
							Interpreter.Vars.Remove(temporaryParameters.Last()); // remove last temp var
							temporaryParameters.RemoveAt(temporaryParameters.Count - 1);
						}

						temporaryParameters.Add(temporaryName); // add the temporary var we just created

					}
				}
			}
			finally
			{
				foreach (var temp in temporaryParameters)
				{
					if (Interpreter.Vars.ContainsKey(temp))
						Interpreter.Vars.Remove(temp);
				}
			}

			return evaluatedValues.Last();
		}

		public static Type GetType(string identifier)
		{
			Type lastType = null;
			while (identifier != "")
			{
				int closingParenthesis = Utils.GetClosingParenthesis(identifier);

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