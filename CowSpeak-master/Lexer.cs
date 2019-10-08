using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.IO;

namespace CowSpeak{
	public class Lexer {
		public List< TokenLine > Lines = new List< TokenLine >();

		public int findClosingBracket(int start){
			int nextSkips = 0; // skip bracket(s) after next Conditional (for nested conditionals)
			for (int j = start + 1; j < Lines.Count; j++){
				if (Lines[j].tokens.Count < 1)
					continue;

				if (Lines[j].tokens[0].type.ToString().IndexOf("Conditional") != -1)
					nextSkips++; // there is a nested conditional, skip the next bracket

				if (Lines[j].tokens[0].type == TokenType.EndBracket){
					if (nextSkips > 0)
						nextSkips--;
					else
						return j;
				}
			}

			CowSpeak.FATAL_ERROR("Conditional is missing an ending curly bracket");
			return -1;
		}

		public static Token ParseToken(string token, bool _throw = true){
			token = token.Replace("True", "1").Replace("False", "0");

			foreach (VarType type in VarType.GetTypes()){
				if (type.Name == token){
					return new Token(TokenType.TypeIdentifier, token);
				}
			} // loop through all language defined types and check for a match

			if (token[0] == '\"' && token[token.Length - 1] == '\"')
				return new Token(TokenType.String, token.Replace(((char)0x1f).ToString(), " ").Substring(1, token.Replace(((char)0x1f).ToString(), " ").Length - 2));
			else if (token[0] == '\'' && token[token.Length - 1] == '\'' && token.Length == 3)
				return new Token(TokenType.Character, token[token.Length - 2].ToString());
			else if (token == "}")
				return new Token(TokenType.EndBracket, token);
			else if (token.IndexOf("(") != -1 && token[token.Length - 1] == ')')
				return new Token(TokenType.FunctionCall, token);
			else if (token.IndexOf("if(") == 0 && token[token.Length - 1] == '{' && token[token.Length - 2] == ')')
				return new Token(TokenType.IfConditional, token);
			else if (token.IndexOf("while(") == 0 && token[token.Length - 1] == '{' && token[token.Length - 2] == ')')
				return new Token(TokenType.WhileConditional, token);
			else if (token.IndexOf("loop(") == 0 && token[token.Length - 1] == '{' && token[token.Length - 2] == ')')
				return new Token(TokenType.LoopConditional, token);
			else if (token == "(" || token == ")")
				return new Token(TokenType.ParenthesesOperator, token);
			else if (token == "-")
				return new Token(TokenType.SubtractOperator, token);
			else if (Utils.IsDigitsOnly(token))
				return new Token(TokenType.Number, token);
			else if (token == "+")
				return new Token(TokenType.AddOperator, token);
			else if (token == "*")
				return new Token(TokenType.MultiplyOperator, token);
			else if (token == "/")
				return new Token(TokenType.DivideOperator, token);
			else if (token == "^")
				return new Token(TokenType.PowerOperator, token);
			else if (token == "%")
				return new Token(TokenType.ModOperator, token);
			else if (token == "=")
				return new Token(TokenType.EqualOperator, token);
			else if (Utils.IsLettersOnly(token))
				return new Token(TokenType.VariableIdentifier, token);

			if (_throw)
				CowSpeak.FATAL_ERROR("Unknown identifier: " + token.Replace(((char)0x1f).ToString(), " "));

			return null;
		}

		public static List < Token > ParseLine(string line, bool _throw = true) {
			if (string.IsNullOrWhiteSpace(line))
				return new List< Token >(); // don't parse empty line

			bool betweenQuotes = false;
			bool betweenParenthesis = false;
			for (int i = 0; i < line.Length; i++){
				if (line[i] == '\"')
					betweenQuotes = !betweenQuotes;
				else if (line[i] == '(')
					betweenParenthesis = true;
				else if (line[i] == ')')
					betweenParenthesis = false;

				if (betweenQuotes && line[i] == ' ')
					line = line.Remove(i, 1).Insert(i, ((char)0x1f).ToString());
				else if (betweenParenthesis && line[i] == ' ')
					line = line.Remove(i, 1).Insert(i, ((char)0x1D).ToString());
			}

			List< string > splitLine = line.Split(' ').ToList();
			List< Token > ret = new List< Token >();
			List< int > skipList = new List< int >(); // lines to be skipped
			for (int i = 0; i < splitLine.Count; i++) {
				if (string.IsNullOrWhiteSpace(splitLine[i]))
					continue;

				foreach (int skip in skipList){
					if (skip == i)
						continue;
				}

				ret.Add(ParseToken(splitLine[i], _throw));
			}

			return ret;
		}

		public Lexer(List< string > fileLines, bool shouldDebug, int currentLineOffset = 0) {
			for (int i = 0; i < fileLines.Count; i++) {
				CowSpeak.currentLine = i + 1 + currentLineOffset;

				fileLines[i] = fileLines[i].Replace(@"\n", Environment.NewLine).Replace("True", "1").Replace("False", "0").Replace("	", ""); // \n is not interpreted as a newline in strings & support for setting booleans using true and false

				while (fileLines[i].IndexOf("#") != -1){
					int pos = fileLines[i].IndexOf("#");
					if (Utils.isBetween(fileLines[i], pos, '"', '"') || Utils.isBetween(fileLines[i], pos, '\'', '\'')){
						StringBuilder fileLine = new StringBuilder(fileLines[i]);
						fileLine[pos] = (char)0x1f;
						fileLines[i] = fileLine.ToString(); 
						continue;
						// replace with placeholder temporarily to prevent while loop forever
					}

					fileLines[i] = fileLines[i].Remove(pos, fileLines[i].Length - pos);
				} // get rid of all '#' and anything after it (but it cannot be enclosed in quotes or apostrophes)

				fileLines[i] = fileLines[i].Replace(((char)0x1f).ToString(), "#"); // replace placeholders back with comment token

				if (string.IsNullOrWhiteSpace(fileLines[i])){
					Lines.Add(new TokenLine(new List<Token>()));
					continue;
				} // no need to parse or evaluate empty line

				Lines.Add(new TokenLine(ParseLine(fileLines[i])));
			}

			for (int i = 0; i < fileLines.Count; i++){
				CowSpeak.currentLine = i + 1 + currentLineOffset;

				if (Lines[i].tokens.Count > 0 && Lines[i].tokens[0].type == TokenType.IfConditional){
					int endingBracket = findClosingBracket(i);

					if (new Conditional(Lines[i].tokens[0].identifier).Evaluate()){
						RestrictedScope scope = new RestrictedScope();

						new Lexer(Utils.GetContainedLines(Lines, endingBracket, i), shouldDebug, i + 1);

						scope.End();
					}

					i = endingBracket; // IfConditional is over, skip to end of brackets to prevent continedLines to be executed again
				}
				else if (Lines[i].tokens.Count > 0 && Lines[i].tokens[0].type == TokenType.WhileConditional){
					int endingBracket = findClosingBracket(i);

					Conditional whileStatement = new Conditional(Lines[i].tokens[0].identifier);
					
					while (whileStatement.Evaluate()){
						RestrictedScope scope = new RestrictedScope();

						new Lexer(Utils.GetContainedLines(Lines, endingBracket, i), shouldDebug, i + 1);

						scope.End();
					}

					i = endingBracket; // while loop is over, skip to end of brackets to prevent continedLines to be executed again
				}
				else if (Lines[i].tokens.Count > 0 && Lines[i].tokens[0].type == TokenType.LoopConditional){
					int endingBracket = findClosingBracket(i);

					string usage = Lines[i].tokens[0].identifier;
					Any[] loopParams = Function.parseParameters(usage.Substring(usage.IndexOf("("), usage.LastIndexOf(")") - usage.IndexOf("(") + 1));

					if (loopParams.Length < 2)
						CowSpeak.FATAL_ERROR("Incorrect number of parameters for LoopConditional (" + loopParams.Length + ") - ");

					string varName = loopParams[0].Get().ToString();
					int count = (int)loopParams[1].Get();

					CowSpeak.Vars.Add(new Variable(VarType.Int, varName));

					for (int p = 0; p < count; p++){
						RestrictedScope scope = new RestrictedScope();

						CowSpeak.getVariable(varName).Set(p);

						new Lexer(Utils.GetContainedLines(Lines, endingBracket, i), shouldDebug, i + 1);

						scope.End();
					}

					for (int p = 0; p < CowSpeak.Vars.Count; p++){
						if (CowSpeak.Vars[p].Name == varName){
							CowSpeak.Vars.RemoveAt(p);
							break;
						}
					} // delete the variable after loop is done

					i = endingBracket; // loop is over, skip to end of brackets to prevent continedLines getting executed again
				}

				if (shouldDebug){
					Console.WriteLine("\nLine " + (i + 1) + ": ");
					foreach (var token in Lines[i].tokens){
						Console.WriteLine(token.type.ToString() + " - " + token.identifier);
					}
				}

				if (Lines[i].tokens.Count >= 2 && Lines[i].tokens[0].type == TokenType.VariableIdentifier && Lines[i].tokens[1].type == TokenType.VariableIdentifier) // first token is interpreted as a variable because the type does not exist
					CowSpeak.FATAL_ERROR("Type '" + Lines[i].tokens[0].identifier + "' does not exist");

				bool shouldBeSet = false; // the most recent variable in list should be set after exec
				if (Lines[i].tokens.Count >= 3 && Lines[i].tokens[0].type == TokenType.TypeIdentifier && Lines[i].tokens[1].type == TokenType.VariableIdentifier && Lines[i].tokens[2].type == TokenType.EqualOperator){
					CowSpeak.Vars.Add(new Variable(VarType.GetType(Lines[i].tokens[0].identifier), Lines[i].tokens[1].identifier));
					shouldBeSet = true;
				} // variable must be created before exec is called so that it may be accessed

				Any retVal = Lines[i].Exec(); // Execute line

				if (shouldBeSet)
					CowSpeak.Vars[CowSpeak.Vars.Count - 1].byteArr = retVal.byteArr;
				else if (Lines[i].tokens.Count >= 2 && Lines[i].tokens[0].type == TokenType.VariableIdentifier && Lines[i].tokens[1].type == TokenType.EqualOperator){
					if (!CowSpeak.isVarDefined(Lines[i].tokens[0].identifier)){
						CowSpeak.FATAL_ERROR("Variable '" + Lines[i].tokens[0].identifier + "' must be defined before it can be set");
					}

					for (int v = 0; v < CowSpeak.Vars.Count; v++){
						if (Lines[i].tokens[0].identifier == CowSpeak.Vars[v].Name)
							CowSpeak.Vars[v].byteArr = retVal.byteArr;
					} // using getVariable does not work for this
				} // type is not specified, var must be defined
			}
		}
	}
}