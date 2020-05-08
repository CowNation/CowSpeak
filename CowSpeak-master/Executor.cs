using System.Collections.Generic;
using System.Linq;

namespace CowSpeak
{
	public static class Executor
	{
		public struct TokenLocation
		{
			public int LineIndex, TokenIndex;

			public TokenLocation(int LineIndex, int TokenIndex)
			{
				this.LineIndex = LineIndex;
				this.TokenIndex = TokenIndex;
			}
		}

		public static TokenLocation GetClosingBracket(List< Line > Lines, int start)
		{
			int skips = 0; // number of EndBracket(s) to skip
			for (int j = start; j < Lines.Count; j++)
			{
				for (int i = 0; i < Lines[j].Count; i++)
				{
					Token token = Lines[j][i];
		
					if (token.type == TokenType.StartBracket)
						skips++;
					else if (token.type == TokenType.EndBracket)
					{
						skips--;

						if (skips <= 0)
							return new TokenLocation(j, i);
					}
				}
			}

			throw new Exception("StartBracket is missing an EndBracket");
		}

		public static Any Execute(List< Line > Lines, int CurrentLineOffset = 0, bool isNestedInFunction = false, bool isNestedInConditional = false)
		{
			for (int i = 0; i < Lines.Count; i++)
			{
				CowSpeak.CurrentLine = i + 1 + CurrentLineOffset;

				if (Lines[i].FirstOrDefault() != null && Lines[i].FirstOrDefault().type == TokenType.ReturnStatement)
				{
					if (isNestedInFunction)
					{
						if (Lines[i].Count < 2)
							return null;

						return new Line(new List<Token>{Lines[i][1]}).Exec();
					}
					else
						throw new Exception("ReturnStatement must be located inside of a FunctionDefinition");
				}

				if (Lines[i].HasFunctionDefinition || Lines[i].HasConditional)
				{
					int ItemIndex = -1; // index of the token that the definition/conditional appears at
					if (Lines[i].HasFunctionDefinition)
						ItemIndex = 1;
					else
						ItemIndex = 0;

					bool NextLineExists = Lines.IsIndexValid(i + 1);
					bool HasPrecedingBracket = Lines[i].IsIndexValid(ItemIndex + 1) && Lines[i][ItemIndex + 1].type == TokenType.StartBracket;
					bool NextLineHasBracket = NextLineExists && Lines[i + 1].FirstOrDefault() != null && Lines[i + 1].FirstOrDefault().type == TokenType.StartBracket;
					if (!HasPrecedingBracket && !NextLineHasBracket)
						throw new Exception("FunctionDefinition or Conditional is missing a preceding StartBracket");

					TokenLocation StartBracket = new TokenLocation(i, ItemIndex + 1); // index of the line that the StartBracket appears at
					if (NextLineHasBracket)
						StartBracket = new TokenLocation(i + 1, 0);

					TokenLocation EndingBracket = GetClosingBracket(Lines, i);

					if (Lines[i].HasFunctionDefinition)
					{
						if (isNestedInFunction || isNestedInConditional)
							throw new Exception("Function cannot be defined inside of a function or conditional");

						string usage = Lines[i][1].identifier.Replace(((char)0x1D).ToString(), " ");

						usage = usage.Substring(0, usage.Length - 1); // remove )
						string dName = usage.Substring(0, usage.IndexOf("(")); // text before first '('

						var DefinitionLines = Utils.GetContainedLines(Lines, EndingBracket, StartBracket);
						CowSpeak.Functions.Create(new UserFunction(dName, DefinitionLines, UserFunction.ParseDefinitionParams(usage.Substring(usage.IndexOf("("))), Utils.GetType(Lines[i][0].identifier), Lines[i][0].identifier + " " + usage + ")", i));
					}
					else if (Lines[i].HasConditional)
					{
						List< Line > ContainedLines = Utils.GetContainedLines(Lines, EndingBracket, StartBracket);

						if (Lines[i][0].type == TokenType.IfConditional)
						{

							if (new Conditional(Lines[i][0].identifier).EvaluateExpression())
							{
								Scope scope = new Scope();

								Execute(ContainedLines, i + 1 + CurrentLineOffset, isNestedInFunction, true);

								scope.End();
							}
						}
						else if (Lines[i][0].type == TokenType.ElseConditional)
						{
							int parentIf = -1;

							if (i == 0 || (Lines[i - 1].Count > 0 && Lines[i - 1][0].type != TokenType.EndBracket))
								throw new Exception("ElseConditional must immediately precede an EndBracket");

							for (int j = 0; j < i; j++)
							{
								if (Lines[j].Count > 0 && Lines[j][0].type == TokenType.IfConditional && GetClosingBracket(Lines, j).LineIndex == i - 1)
								{
									parentIf = j;
									break;
								}
							}

							if (parentIf == -1)
								throw new Exception("ElseConditional isn't immediately preceding an EndBracket");

							if (!new Conditional(Lines[parentIf][0].identifier).EvaluateExpression())
							{
								Scope scope = new Scope();

								Execute(ContainedLines, i + 1 + CurrentLineOffset, isNestedInFunction, true);

								scope.End();
							}
						}
						else if (Lines[i][0].type == TokenType.WhileConditional)
						{
							Conditional whileStatement = new Conditional(Lines[i][0].identifier);
							
							while (whileStatement.EvaluateExpression())
							{
								Scope scope = new Scope();

								Execute(ContainedLines, i + 1 + CurrentLineOffset, isNestedInFunction, true);

								scope.End();
							}
						}
						else if (Lines[i][0].type == TokenType.LoopConditional)
						{
							string usage = Lines[i][0].identifier;
							Any[] loopParams = StaticFunction.ParseParameters(usage.Substring(usage.IndexOf("("), usage.LastIndexOf(")") - usage.IndexOf("(") + 1));

							StaticFunction Loop = new StaticFunction("Loop", null, Type.Void, new Parameter[]{ new Parameter(Type.String, "IndexVariableName"), new Parameter(Type.Integer, "StartAt"), new Parameter(Type.Integer, "EndAt") });
							Loop.CheckParameters(loopParams.ToList()); // throws errors if given parameters are bad

							StaticFunction.Functions.Loop(ContainedLines, i, CurrentLineOffset, isNestedInFunction, loopParams[0].Value.ToString(), (int)loopParams[1].Value, (int)loopParams[2].Value);
						}
					}

					// skip to after the end of definition
					i = EndingBracket.LineIndex;
					continue;
				}

				if (i >= Lines.Count)
					break;

				bool shouldBeSet = false; // topmost variable in list should be set after exec
				if (Lines[i].Count >= 2 && Lines[i][0].type == TokenType.TypeIdentifier && Lines[i][1].type == TokenType.VariableIdentifier)
				{
					CowSpeak.Vars.Create(new Variable(Type.GetType(Lines[i][0].identifier), Lines[i][1].identifier));

					if (Lines[i].Count >= 3 && Lines[i][2].type == TokenType.EqualOperator)
						shouldBeSet = true;
				} // variable must be created before exec is called so that it may be accessed

				Any retVal = Lines[i].Exec(); // Execute line

				if (Lines[i].Count >= 3 && Lines[i][1].type == TokenType.VariableIdentifier && Lines[i][2].type == TokenType.EqualOperator && !Conversion.IsCompatible(CowSpeak.Vars[Lines[i][1].identifier].Type, retVal.Type))
					throw new Exception("Cannot set '" + Lines[i][1].identifier + "', type '" + CowSpeak.Vars[Lines[i][1].identifier].Type.Name + "' is incompatible with '" + retVal.Type.Name + "'"); // check if types are compatible
				else if (Lines[i].Count >= 2 && Lines[i][0].type == TokenType.VariableIdentifier && Lines[i][1].type == TokenType.EqualOperator && !Conversion.IsCompatible(CowSpeak.Vars[Lines[i][0].identifier].Type, retVal.Type))
					throw new Exception("Cannot set '" + Lines[i][0].identifier + "', type '" + CowSpeak.Vars[Lines[i][0].identifier].Type.Name + "' is incompatible with '" + retVal.Type.Name + "'"); // check if types are compatible

				if (shouldBeSet)
				{
					CowSpeak.Vars[Lines[i][1].identifier].bytes = retVal.bytes;
					var val = CowSpeak.Vars[Lines[i][1].identifier].Value; // Do this in case there was an error when setting bytes
				}
				else if (Lines[i].Count >= 2 && Lines[i][0].type == TokenType.VariableIdentifier && Lines[i][1].type == TokenType.EqualOperator)
				{
					if (!CowSpeak.Vars.ContainsKey(Lines[i][0].identifier))
						throw new Exception("Variable '" + Lines[i][0].identifier + "' must be defined before it can be set"); // var not found

					CowSpeak.Vars[Lines[i][0].identifier].bytes = retVal.bytes;
					var val = CowSpeak.Vars[Lines[i][0].identifier].Value; // In case there was an error when setting bytes
				} // type is not specified, var must already be defined
			}

			return null;
		}
	}
}