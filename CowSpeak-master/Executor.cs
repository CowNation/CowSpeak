using CowSpeak.Exceptions;
using System;
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

			throw new BaseException("StartBracket is missing an EndBracket");
		}

		public static Any Execute(List< Line > lines, int currentLineOffset = 0, bool nestedInFunction = false, bool nestedInConditional = false)
		{
			for (int i = 0; i < lines.Count; i++)
			{
				Interpreter.CurrentLine = i + 1 + currentLineOffset;

				// if this line starts with a ReturnStatement
				if (lines[i].FirstOrDefault() != null && lines[i].FirstOrDefault().type == TokenType.ReturnStatement)
				{
					if (nestedInFunction)
					{
						// no value is returned
						if (lines[i].Count < 2)
							return null;

						return new Line(new List<Token>{ lines[i][1] }).Execute();
					}
					else
						throw new BaseException("ReturnStatement must be located inside of a FunctionDefinition");
				}

				if (lines[i].IsFunctionDefinition || lines[i].IsConditional)
				{
					int itemIndex; // index of the token that the definition/conditional appears at
					if (lines[i].IsFunctionDefinition)
						itemIndex = 1;
					else
						itemIndex = 0;

					bool nextLineExists = lines.IsIndexValid(i + 1);
					bool hasPrecedingBracket = lines[i].IsIndexValid(itemIndex + 1) && lines[i][itemIndex + 1].type == TokenType.StartBracket;
					bool nextLineHasBracket = nextLineExists && lines[i + 1].FirstOrDefault() != null && lines[i + 1].FirstOrDefault().type == TokenType.StartBracket;
					if (!hasPrecedingBracket && !nextLineHasBracket)
						throw new BaseException("FunctionDefinition or Conditional is missing a preceding StartBracket");

					TokenLocation startBracket = new TokenLocation(i, itemIndex + 1); // index of the line that the StartBracket appears at
					if (nextLineHasBracket)
						startBracket = new TokenLocation(i + 1, 0);

					TokenLocation endingBracket = GetClosingBracket(lines, i);

					if (lines[i].IsFunctionDefinition)
					{
						if (nestedInFunction || nestedInConditional)
							throw new BaseException("Function cannot be defined inside of a function or conditional");

						string usage = lines[i][1].identifier;
						string dName = usage.Substring(0, usage.IndexOf("(")); // text before first '('

						var definitionLines = Utils.GetContainedLines(lines, endingBracket, startBracket);
						Interpreter.Functions.Create(new UserFunction(dName, definitionLines, UserFunction.ParseDefinitionParams(usage.Substring(usage.IndexOf("("))), Type.GetType(lines[i][0].identifier), lines[i][0].identifier + " " + usage + ")", i));
					}
					else if (lines[i].IsConditional)
					{
						var containedLines = Utils.GetContainedLines(lines, endingBracket, startBracket);

						if (lines[i][0].type == TokenType.IfConditional)
						{
							if (new Conditional(lines[i][0].identifier).EvaluateExpression())
							{
								Scope scope = new Scope();

								Execute(containedLines, i + 1 + currentLineOffset, nestedInFunction, true);

								scope.End();
							}
						}
						else if (lines[i][0].type == TokenType.ElseConditional)
						{
							int parentIf = -1;

							if (i == 0 || (lines[i - 1].Count > 0 && lines[i - 1][0].type != TokenType.EndBracket))
								throw new BaseException("ElseConditional must immediately precede an EndBracket");

							for (int j = 0; j < i; j++)
							{
								if (lines[j].Count > 0 && lines[j][0].type == TokenType.IfConditional && GetClosingBracket(lines, j).LineIndex == i - 1)
								{
									parentIf = j;
									break;
								}
							}

							if (parentIf == -1)
								throw new BaseException("ElseConditional must immediately precede an EndBracket");

							if (!new Conditional(lines[parentIf][0].identifier).EvaluateExpression())
							{
								Scope scope = new Scope();

								Execute(containedLines, i + 1 + currentLineOffset, nestedInFunction, true);

								scope.End();
							}
						}
						else if (lines[i][0].type == TokenType.WhileConditional)
						{
							Conditional whileStatement = new Conditional(lines[i][0].identifier);
							
							while (whileStatement.EvaluateExpression())
							{
								Scope scope = new Scope();

								Execute(containedLines, i + 1 + currentLineOffset, nestedInFunction, true);

								scope.End();
							}
						}
						else if (lines[i][0].type == TokenType.LoopConditional)
						{
							string usage = lines[i][0].identifier;
							Any[] loopParams = BaseFunction.ParseParameters(usage.Substring(usage.IndexOf("("), usage.LastIndexOf(")") - usage.IndexOf("(") + 1));

							// throws errors if given parameters are bad
							BaseFunction.CheckParameters("Loop", 
								new Parameter[] { new Parameter(Types.String, "indexVariableName"), new Parameter(Types.Integer, "start"), new Parameter(Types.Integer, "exclusiveEnd") }, 
								loopParams.ToList());

							Modules.Main.Loop(containedLines, i, currentLineOffset, nestedInFunction, loopParams[0].Value.ToString(), (int)loopParams[1].Value, (int)loopParams[2].Value);
						}
					}

					// skip to after the end of definition
					i = endingBracket.LineIndex;
					continue;
				}

				if (i >= lines.Count)
					break;

				bool shouldBeSet = false; // topmost variable in list should be set after exec
				if (lines[i].Count >= 2 && lines[i][0].type == TokenType.TypeIdentifier && lines[i][1].type == TokenType.VariableIdentifier)
				{
					var varType = Type.GetType(lines[i][0].identifier);
					var varName = lines[i][1].identifier;

					// initializing a variable whose type is a FastStruct
					if (varType is FastStruct)
                    {
						// check if a variable exists with the same name
						if (Interpreter.Vars.ContainsKey(varName))
							throw new BaseException("Variable '" + varName + "' has already been defined");

						List<Variable> memberInstances = new List<Variable>();
						foreach (var member in ((FastStruct)varType).Members)
                        {
							var memberVariable = new Variable(member.Value.Type, varName + "." + member.Key);
							Interpreter.Vars.Add(memberVariable.Name, memberVariable);
							memberInstances.Add(Interpreter.Vars[memberVariable.Name]);
						}
						Interpreter.Vars.Create(new Variable(varType, varName, memberInstances));
					}
					else
						Interpreter.Vars.Create(new Variable(varType, varName));

					if (lines[i].Count >= 3 && lines[i][2].type == TokenType.EqualOperator)
						shouldBeSet = true;
				} // variable must be created before exec is called so that it may be accessed

				Any retVal = lines[i].Execute(); // Execute line

				if (retVal != null)
				{
					if (lines[i].Count >= 3 && lines[i][1].type == TokenType.VariableIdentifier && lines[i][2].type == TokenType.EqualOperator && !Conversion.IsCompatible(Interpreter.Vars[lines[i][1].identifier].Type, retVal.Type))
						throw new ConversionException("Cannot set '" + lines[i][1].identifier + "', type '" + Interpreter.Vars[lines[i][1].identifier].Type.Name + "' is incompatible with type '" + retVal.Type.Name + "'"); // check if types are compatible
					else if (lines[i].Count >= 2 && lines[i][0].type == TokenType.VariableIdentifier && lines[i][1].type == TokenType.EqualOperator && !Conversion.IsCompatible(Interpreter.Vars[lines[i][0].identifier].Type, retVal.Type))
						throw new ConversionException("Cannot set '" + lines[i][0].identifier + "', type '" + Interpreter.Vars[lines[i][0].identifier].Type.Name + "' is incompatible with type '" + retVal.Type.Name + "'"); // check if types are compatible
				}

				if (shouldBeSet)
				{
					Interpreter.Vars[lines[i][1].identifier].Value = retVal == null ? null : retVal.obj;
				}
				else if (lines[i].Count >= 2 && lines[i][0].type == TokenType.VariableIdentifier && lines[i][1].type == TokenType.EqualOperator)
				{
					if (!Interpreter.Vars.ContainsKey(lines[i][0].identifier))
						throw new BaseException("Variable '" + lines[i][0].identifier + "' must be defined before it can be set"); // var not found

					Interpreter.Vars[lines[i][0].identifier].Value = retVal == null ? null : retVal.obj;
				} // type is not specified, var must already be defined
			}

			return null;
		}
	}
}