using System.Collections.Generic;
using System.Linq;

namespace CowSpeak
{
	public static class Executor
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

					CowSpeak.CreateFunction(new UserFunction(dName, Utils.pGetContainedLines(Lines, GetClosingBracket(Lines, i), i), UserFunction.ParseDefinitionParams(usage.Substring(usage.IndexOf("("))), Utils.GetType(Lines[i][0].identifier), Lines[i][0].identifier + " " + usage + ")", i));

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

						string varName = loopParams[0].Get().ToString();
						int start = (int)loopParams[1].Get();
						int end = (int)loopParams[2].Get();

						CowSpeak.Vars.Add(new Variable(Type.Integer, varName));

						for (int p = start; p < end; p++)
						{
							Scope scope = new Scope();

							CowSpeak.GetVariable(varName).Set(p);

							new Lexer(ContainedLines, i + 1 + CurrentLineOffset, isNestedInFunction, true);

							scope.End();
						}

						CowSpeak.Vars.Remove(CowSpeak.GetVariable(varName)); // delete the variable after loop is done

						i = endingBracket; // loop is over, skip to end of brackets to prevent continedLines getting executed again
					}
				}

				if (i >= Lines.Count)
					break;

				if (Lines[i].Count == 2 && Lines[i][0].type == TokenType.DeleteIdentifier && Lines[i][1].type == TokenType.VariableIdentifier)
				{
					Variable target = CowSpeak.GetVariable(Lines[i][1].identifier);
					CowSpeak.Vars.Remove(CowSpeak.GetVariable(target.Name));

					continue; // prevent execution
				} // must handle this before the other lines are evaluated to avoid wrong exceptions

				bool shouldBeSet = false; // topmost variable in list should be set after exec
				if (Lines[i].Count >= 3 && Lines[i][0].type == TokenType.TypeIdentifier && Lines[i][1].type == TokenType.VariableIdentifier && Lines[i][2].type == TokenType.EqualOperator)
				{
					CowSpeak.CreateVariable(new Variable(Type.GetType(Lines[i][0].identifier), Lines[i][1].identifier));

					shouldBeSet = true;
				} // variable must be created before exec is called so that it may be accessed

				Any retVal = Lines[i].Exec(); // Execute line

				if (Lines[i].Count >= 3 && Lines[i][1].type == TokenType.VariableIdentifier && Lines[i][2].type == TokenType.EqualOperator && !Conversion.IsCompatible(CowSpeak.GetVariable(Lines[i][1].identifier).vType, retVal.vType))
					throw new Exception("Cannot set '" + Lines[i][1].identifier + "', type '" + CowSpeak.GetVariable(Lines[i][1].identifier).vType.Name + "' is incompatible with '" + retVal.vType.Name + "'"); // check if types are compatible
				else if (Lines[i].Count >= 2 && Lines[i][0].type == TokenType.VariableIdentifier && Lines[i][1].type == TokenType.EqualOperator && !Conversion.IsCompatible(CowSpeak.GetVariable(Lines[i][0].identifier).vType, retVal.vType))
					throw new Exception("Cannot set '" + Lines[i][0].identifier + "', type '" + CowSpeak.GetVariable(Lines[i][0].identifier).vType.Name + "' is incompatible with '" + retVal.vType.Name + "'"); // check if types are compatible

				if (shouldBeSet)
					CowSpeak.Vars[CowSpeak.Vars.Count - 1].byteArr = retVal.byteArr;
				else if (Lines[i].Count >= 2 && Lines[i][0].type == TokenType.VariableIdentifier && Lines[i][1].type == TokenType.EqualOperator)
				{
					if (CowSpeak.GetVariable(Lines[i][0].identifier, false) == null){
						throw new Exception("Variable '" + Lines[i][0].identifier + "' must be defined before it can be set");
					} // var not found

					for (int v = 0; v < CowSpeak.Vars.Count; v++)
						if (Lines[i][0].identifier == CowSpeak.Vars[v].Name)
							CowSpeak.Vars[v].byteArr = retVal.byteArr;
				} // type is not specified, var must already be defined
			}
		}
	}
}