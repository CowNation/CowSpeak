using System.Collections.Generic;
using System.Linq;

namespace CowSpeak
{
	internal static class FunctionChain
	{
		public static bool IsChain(string token)
		{
			bool atLeastOneChild = false;
			for (int i = 0; i < token.OccurrencesOf("."); i++)
			{
				if (i == 0)
					continue;

				int index = token.OrdinalIndexOf(".", i);
				if (index - 1 >= 0 && token[index - 1] == ')')
				{
					atLeastOneChild = true;
					break;
				}
			}

			return atLeastOneChild && 
			token.OccurrencesOf("(") > 1 &&
			token.OccurrencesOf(")") > 1 &&
			token.OccurrencesOf("(") == token.OccurrencesOf(")") && 
			token.IndexOf(".") < token.IndexOf("(") && 
			token.IndexOf(".") != -1 &&
			token.OccurrencesOf(".") > 1;
		}

		public static Any Evaluate(string identifier)
		{
			List< Any > EvaluatedValues = new List< Any >();
			int Funcs = identifier.OccurrencesOf("(");
			for (int i = 0; i < Funcs; i++)
			{
				int End = identifier.IndexOf(")", i);
				string FuncIdentifier = identifier.Substring(0, End + 1);

				FunctionBase ChainFunc = CowSpeak.GetFunction(FuncIdentifier);
				EvaluatedValues.Add(ChainFunc.Execute(FuncIdentifier));

				CowSpeak.CreateVariable(new Variable(EvaluatedValues.Last().Type, "temp_" + i, EvaluatedValues.Last().Value));
				identifier = ("temp_" + i) + identifier.Substring(End + 1);

				var LastTempVar = CowSpeak.GetVariable("temp_" + (i - 1), false);
				if (LastTempVar != null)
					CowSpeak.Vars.Remove(LastTempVar); // remove last temp var
			}
			CowSpeak.Vars.Remove(CowSpeak.GetVariable("temp_" + (Funcs - 1), false));
				
			return EvaluatedValues.Last();
		}

		public static Type GetType(string identifier)
		{
			Type LastType = null;
			while (identifier != "")
			{
				int DotIndex = identifier.IndexOf(".");
				int End = identifier.IndexOf(")");

				string Object = identifier.Substring(0, DotIndex);
				string FuncIdentifier = identifier.Substring(DotIndex, End - DotIndex + 1);

				var type = Type.GetType(Object, false);
				if (type != null)
				{
					FuncIdentifier = type.Name + FuncIdentifier;
					LastType = CowSpeak.GetFunction(FuncIdentifier, false).type;
				}

				var _var = CowSpeak.GetVariable(Object, false);
				if (_var != null)
				{
					FuncIdentifier = _var.Type.Name + FuncIdentifier;
					LastType = CowSpeak.GetFunction(FuncIdentifier, false).type;
				}

				var func = CowSpeak.GetFunction(Object + FuncIdentifier, false);
				if (func != null)
					LastType = func.type;

				if (End + 1 >= identifier.Length)
					break;

				identifier = LastType.Name + identifier.Substring(End + 1);
				continue;
			}

			return LastType;
		}
	}
}