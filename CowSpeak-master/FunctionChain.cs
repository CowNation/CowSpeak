using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace CowSpeak
{
	internal static class FunctionChain
	{
		public static bool IsChain(string token)
		{
			return Regex.IsMatch(token, "(\\w+(\\(.*\\)\\.|\\.\\w+(\\(.*\\)\\.)))+(\\w+\\(.*\\))") && Utils.GetInitialClosingParenthesis(token) != token.Length - 1;
		}

		public static Any Evaluate(string identifier)
		{
			List< Any > EvaluatedValues = new List< Any >();
			int Funcs = identifier.OccurrencesOf("(");
			for (int i = 0; i < Funcs; i++)
			{
				int End = identifier.IndexOf(")", i);
				string FuncIdentifier = identifier.Substring(0, End + 1);

				FunctionBase ChainFunc = CowSpeak.Functions.Get(FuncIdentifier);
				EvaluatedValues.Add(ChainFunc.Execute(FuncIdentifier));

				CowSpeak.Vars.Create(new Variable(EvaluatedValues.Last().Type, "temp_" + i, EvaluatedValues.Last().Value));
				identifier = ("temp_" + i) + identifier.Substring(End + 1);

				var LastTempVar = CowSpeak.Vars.Get("temp_" + (i - 1), false);
				if (LastTempVar != null)
					CowSpeak.Vars.Remove(LastTempVar); // remove last temp var
			}
			CowSpeak.Vars.Remove(CowSpeak.Vars.Get("temp_" + (Funcs - 1), false));
				
			return EvaluatedValues.Last();
		}

		public static Type GetType(string identifier)
		{
			Type LastType = null;
			while (identifier != "")
			{
				int ClosingParenthesis = Utils.GetInitialClosingParenthesis(identifier);

				string FuncIdentifier = identifier.Substring(0, ClosingParenthesis + 1);

				var func = CowSpeak.Functions.Get(FuncIdentifier, false);
				if (func != null)
					LastType = func.type;

				if (ClosingParenthesis + 1 >= identifier.Length || LastType == null)
					break;

				identifier = LastType.Name + identifier.Substring(ClosingParenthesis + 1);
				continue;
			}

			return LastType;
		}
	}
}