using System.Collections.Generic;
using System.Linq;

namespace CowSpeak
{
	internal static class Executor
	{
		public static int GetClosingBracket(List< Line > Lines, int start)
		{
			int nextSkips = 0; // skip bracket(s) after next Conditional (for nested conditionals)
			for (int j = start + 1; j < Lines.Count; j++)
			{
				if (Lines[j].Count < 1)
					continue;

				if (Lines[j][0].type.ToString().IndexOf("Conditional") != -1 || (Lines[j].Count > 2 && Lines[j][1].type == TokenType.FunctionCall && Lines[j][0].type == TokenType.TypeIdentifier && Lines[j][2].type == TokenType.StartBracket))
					nextSkips++; // there is a nested conditional, skip the next bracket

				if (Lines[j][0].type == TokenType.EndBracket)
				{
					if (nextSkips > 0)
						nextSkips--;
					else
						return j;
				}
			}

			throw new Exception("Conditional or function is missing an ending curly bracket");
		}

		public static void Execute(List< Line > Lines, int CurrentLineOffset = 0, bool isNestedInFunction = false, bool isNestedInConditional = false)
		{
			for (int i = 0; i < Lines.Count; i++)
			{
				CowSpeak.CurrentLine = i + 1 + CurrentLineOffset;

				if (Lines[i].Count >= 1 && Lines[i][0].type == TokenType.ReturnStatement)
				{
					if (isNestedInFunction)
						return; // ReturnStatement to be handled by UserFunction.ExecuteLines
					else
						throw new Exception("ReturnStatement must be located inside of a FunctionDefinition");
				}

				if (Lines[i].Count > 1 && Lines[i][0].type == TokenType.TypeIdentifier && Lines[i][1].type == TokenType.FunctionCall)
				{
					if (Lines[i].Count < 3 || Lines[i][2].type != TokenType.StartBracket)
						throw new Exception("StartBracket must immediately precede a function definition");

					if (isNestedInFunction || isNestedInConditional)
						throw new Exception("Function cannot be defined inside of a function or conditional");

					string usage = Lines[i][1].identifier.Replace(((char)0x1D).ToString(), " ");

					usage = usage.Substring(0, usage.Length - 1); // remove )
					string dName = usage.Substring(0, usage.IndexOf("(")); // text before first '('

					CowSpeak.Functions.Create(new UserFunction(dName, Utils.pGetContainedLines(Lines, GetClosingBracket(Lines, i), i), UserFunction.ParseDefinitionParams(usage.Substring(usage.IndexOf("("))), Utils.GetType(Lines[i][0].identifier), Lines[i][0].identifier + " " + usage + ")", i));

					i = GetClosingBracket(Lines, i); // skip to end of definition
				}
				else if (Lines[i].Count > 0 && Lines[i][0].type.ToString().IndexOf("Conditional") != -1)
				{
					if (Lines[i].Count < 2 || Lines[i][1].type != TokenType.StartBracket)
						throw new Exception("StartBracket must immediately precede a conditional");

					int endingBracket = GetClosingBracket(Lines, i);
					List< string > ContainedLines = Utils.GetContainedLines(Lines, endingBracket, i);

					if (Lines[i][0].type == TokenType.IfConditional)
					{

						if (new Conditional(Lines[i][0].identifier).EvaluateBoolean())
						{
							Scope scope = new Scope();

							new Lexer(ContainedLines, i + 1 + CurrentLineOffset, isNestedInFunction, true);

							scope.End();
						}

						i = endingBracket; // IfConditional is over, skip to end of brackets to prevent continedLines to be executed again
					}
					else if (Lines[i][0].type == TokenType.ElseConditional)
					{
						int parentIf = -1;

						if (i == 0 || (Lines[i - 1].Count > 0 && Lines[i - 1][0].type != TokenType.EndBracket))
							throw new Exception("ElseConditional isn't immediately preceding an EndBracket");

						for (int j = 0; j < i; j++)
						{
							if (Lines[j].Count > 0 && Lines[j][0].type == TokenType.IfConditional && GetClosingBracket(Lines, j) == i - 1)
							{
								parentIf = j;
								break;
							}
						}

						if (parentIf == -1)
							throw new Exception("ElseConditional isn't immediately preceding an EndBracket");

						if (!new Conditional(Lines[parentIf][0].identifier).EvaluateBoolean()){
							Scope scope = new Scope();

							new Lexer(ContainedLines, i + 1 + CurrentLineOffset, isNestedInFunction, true);

							scope.End();
						}

						i = endingBracket; // ElseConditional is over, skip to end of brackets to prevent continedLines to be executed again
					}
					else if (Lines[i][0].type == TokenType.WhileConditional)
					{
						Conditional whileStatement = new Conditional(Lines[i][0].identifier);
						
						while (whileStatement.EvaluateBoolean())
						{
							Scope scope = new Scope();

							new Lexer(ContainedLines, i + 1 + CurrentLineOffset, isNestedInFunction, true);

							scope.End();
						}

						i = endingBracket; // while loop is over, skip to end of brackets to prevent continedLines to be executed again
					}
					else if (Lines[i][0].type == TokenType.LoopConditional)
					{
						string usage = Lines[i][0].identifier;
						Any[] loopParams = StaticFunction.ParseParameters(usage.Substring(usage.IndexOf("("), usage.LastIndexOf(")") - usage.IndexOf("(") + 1));

						StaticFunction Loop = new StaticFunction("Loop", null, Type.Void, new Parameter[]{ new Parameter(Type.String, "IndexVariableName"), new Parameter(Type.Integer, "StartAt"), new Parameter(Type.Integer, "EndAt") });
						Loop.CheckParameters(loopParams.ToList()); // throws errors if given parameters are bad

						string varName = loopParams[0].Value.ToString();
						//System.Console.WriteLine(loopParams[1].Type.Name + "," + loopParams[2].Type.Name);				
						long start = loopParams[1].Type == Type.Integer64 ? (long)loopParams[1].Value : (int)loopParams[1].Value;
						long end = loopParams[2].Type == Type.Integer64 ? (long)loopParams[2].Value : (int)loopParams[2].Value;

						CowSpeak.Vars.Create(new Variable(Type.Integer, varName));

						for (long p = start; p < end; p++)
						{
							Scope scope = new Scope();

							CowSpeak.Vars.Get(varName).Value = p;

							new Lexer(ContainedLines, i + 1 + CurrentLineOffset, isNestedInFunction, true);

							scope.End();
						}

						CowSpeak.Vars.Remove(CowSpeak.Vars.Get(varName)); // delete the variable after loop is done

						i = endingBracket; // loop is over, skip to end of brackets to prevent continedLines getting executed again
					}
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

				if (Lines[i].Count >= 3 && Lines[i][1].type == TokenType.VariableIdentifier && Lines[i][2].type == TokenType.EqualOperator && !Conversion.IsCompatible(CowSpeak.Vars.Get(Lines[i][1].identifier).Type, retVal.Type))
					throw new Exception("Cannot set '" + Lines[i][1].identifier + "', type '" + CowSpeak.Vars.Get(Lines[i][1].identifier).Type.Name + "' is incompatible with '" + retVal.Type.Name + "'"); // check if types are compatible
				else if (Lines[i].Count >= 2 && Lines[i][0].type == TokenType.VariableIdentifier && Lines[i][1].type == TokenType.EqualOperator && !Conversion.IsCompatible(CowSpeak.Vars.Get(Lines[i][0].identifier).Type, retVal.Type))
					throw new Exception("Cannot set '" + Lines[i][0].identifier + "', type '" + CowSpeak.Vars.Get(Lines[i][0].identifier).Type.Name + "' is incompatible with '" + retVal.Type.Name + "'"); // check if types are compatible

				if (shouldBeSet)
				{
					CowSpeak.Vars.Last().bytes = retVal.bytes;

					try
					{
						var val = CowSpeak.Vars.Last().Value; // Do this in case there was an overflow error
					}
					catch
					{
						System.Console.WriteLine("caught that bugger");
					}
				}
				else if (Lines[i].Count >= 2 && Lines[i][0].type == TokenType.VariableIdentifier && Lines[i][1].type == TokenType.EqualOperator)
				{
					if (CowSpeak.Vars.Get(Lines[i][0].identifier, false) == null){
						throw new Exception("Variable '" + Lines[i][0].identifier + "' must be defined before it can be set");
					} // var not found

					for (int v = 0; v < CowSpeak.Vars.Count; v++)
					{
						if (Lines[i][0].identifier == CowSpeak.Vars[v].Name)
						{
							CowSpeak.Vars[v].bytes = retVal.bytes;
							var val = CowSpeak.Vars[v].Value; // Do this in case there was an overflow error
						}
					}
				} // type is not specified, var must already be defined
			}
		}
	}
}