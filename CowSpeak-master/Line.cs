using System.Collections.Generic;

namespace CowSpeak
{
	internal class Line : List< Token >
	{
		public Line(List< Token > tokens) : base(tokens)
		{

		}

		private static Any FixCharChain(List< Token > toEval, int i, string identifier)
		{
			if ((toEval[i].type == TokenType.Character || Utils.IsCharable(toEval[i])) && i == toEval.Count - 1)
			{
				Any result = new Any();
				result.Type = Type.Character;

				if (toEval[i].type == TokenType.Character)
					result.Value = toEval[i].identifier != "" ? toEval[i].identifier[0] : (char)0;
				else if (toEval[i].type == TokenType.FunctionCall)
					result.Value = (char)CowSpeak.GetFunction(identifier).Execute(identifier).Value;
				else if (toEval[i].type == TokenType.FunctionChain)
					result.Value = (char)FunctionChain.Evaluate(identifier).Value;
				else
					result.Value = (char)CowSpeak.GetVariable(toEval[i].identifier).Value; // stringable variable

				return result;
			}

			return null;
		} // don't start chain starting with a char if it has no friends

		private static Any TryStrChain(List< Token > toEval, int i, string identifier)
		{
			if (toEval[i].type == TokenType.String || Utils.IsStringable(toEval[i]) || toEval[i].type == TokenType.Character || Utils.IsCharable(toEval[i]))
			{
				// who wants some spaghetti?
				Any result = new Any();
				result.Type = Type.String;
				List< string > additors = new List< string >();

				if (toEval[i].type == TokenType.String || toEval[i].type == TokenType.Character)
					additors.Add(toEval[i].identifier);
				else if (toEval[i].type == TokenType.VariableIdentifier)
					additors.Add(CowSpeak.GetVariable(toEval[i].identifier).Value.ToString()); // stringable variable
				else if (toEval[i].type == TokenType.FunctionCall)
					additors.Add(CowSpeak.GetFunction(identifier).Execute(identifier).Value.ToString());
				else if (toEval[i].type == TokenType.FunctionChain)
					additors.Add(FunctionChain.Evaluate(identifier).Value.ToString());
				else
					throw new Exception("An unknown error has occured in 'TryStrChain' (Could not determine type in StringChain)");

				int index = i;
				while (true)
				{
					index++;
					if (index != i + 1)
						index++;
					
					if (!toEval.IsIndexValid(index + 1) || !toEval.IsIndexValid(index) || toEval[index].type != TokenType.AddOperator)
						break;
					
					if (toEval[index + 1].type == TokenType.VariableIdentifier)
						additors.Add(CowSpeak.GetVariable(toEval[index + 1].identifier).Value.ToString());
					else if (toEval[index + 1].type == TokenType.String || toEval[index + 1].type == TokenType.Number || toEval[index + 1].type == TokenType.Character)
						additors.Add(toEval[index + 1].identifier);
					else if (toEval[index + 1].type == TokenType.FunctionCall)
						additors.Add(CowSpeak.GetFunction(toEval[index + 1].identifier).Execute(toEval[index + 1].identifier).Value.ToString()); // stringable func
					else if (toEval[index + 1].type == TokenType.FunctionChain)
						additors.Add(FunctionChain.Evaluate(toEval[index + 1].identifier).Value.ToString());
					else 
						break;
				}

				if (additors.Count == 1)
					result.Value = additors[0];
				else
					result.Value = Utils.AddStrings(additors);

				return result;
			}

			return null;
		}

		public Any Exec()
		{
			List< Token > toEval = GetRange(0, Count); // get a copy

			for (int i = 0; i < Count; i++)
			{
				if (base[i].type == TokenType.FunctionCall)
				{
					if (CowSpeak.GetFunction(base[i].identifier).type == Type.Void){
						if (GetRange(0, Count).IsIndexValid(i - 1) && Utils.IsOperator(base[i-1].type))
							throw new Exception("Cannot perform operation: '" + base[i-1].identifier + "' on void function");
						if (GetRange(0, Count).IsIndexValid(i + 1) && Utils.IsOperator(base[i+1].type))
							throw new Exception("Cannot perform operation: '" + base[i+1].identifier + "' on void function");
					}
				}
			}

			for (int i = 0; i < Count; i++)
			{
				if (base[i].type == TokenType.EqualOperator)
				{
					toEval = GetRange(i + 1, Count - (i + 1));
					break;
				}
			} // remove the equal sign and everything to the left

			if (Count == 2 && base[0].type == TokenType.TypeIdentifier && base[1].type == TokenType.VariableIdentifier)
				return null; // support for uninitialized vars

			string Expression = "";
			for (int i = 0; i < toEval.Count; i++)
			{
				string identifier = toEval[i].identifier;

				Any fixCharChain = FixCharChain(toEval, i, identifier);
				if (fixCharChain != null)
					return fixCharChain;

				Any strChain = TryStrChain(toEval, i, identifier);
				if (strChain != null)
					return strChain;

				if (toEval[i].type == TokenType.VariableIdentifier)
					identifier = CowSpeak.GetVariable(identifier).Value.ToString(); // replace variable name with it's value
				if (toEval[i].type == TokenType.WhileConditional || toEval[i].type == TokenType.IfConditional || toEval[i].type == TokenType.EndBracket)
					continue;
				else if (toEval[i].type == TokenType.FunctionCall)
				{
					FunctionBase func = CowSpeak.GetFunction(identifier);
					Any returned = func.Execute(identifier);
					if (toEval.Count == 1)
						return returned;
					if (returned != null)
						identifier = returned.Value.ToString(); // replace function call with it's return value
					else
						identifier = "";
				}
				else if (toEval[i].type == TokenType.FunctionChain)
				{
					Any EvaluatedChain = FunctionChain.Evaluate(identifier);

					if (toEval.Count == 1)
						return EvaluatedChain;

					identifier = EvaluatedChain.Value.ToString();
				}
				
				Expression += identifier;
			}

			if (Expression.Length == 0)
				return new Any(Type.Integer, 0);

			if (Expression.OccurrencesOf(" ") == 0 && Utils.IsNumber(Expression))
			{
				Any val = new Any();
				if (Expression.IndexOf(".") != -1)
				{
					val.Type = Type.Decimal;
					val.Value = double.Parse(Expression);
				}
				else
				{
					val.Type = Type.Integer64;
					val.Value = long.Parse(Expression);
					if ((long)val.Value < int.MaxValue)
						val.Type = Type.Integer;
				}
				
				return val;
			}

			return new Any(Evaluate.EvaluateExpression(Expression));
		}
	}
}