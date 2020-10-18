using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using CowSpeak.Exceptions;
using System.Reflection;

namespace CowSpeak
{
	public static class Lexer
	{
		static Lexer()
		{
			terms.Sort((x, y) => y.Length.CompareTo(x.Length));
		}

		public static Token ParseToken(string token, bool @throw = true, int index = -1)
		{
			while (token.Length > 0 && token[0] == ' ')
				token = token.Remove(0);

			if (token.Length > 0)
			{
				if (Interpreter.Definitions.ContainsKey(token))
					token = Interpreter.Definitions[token].To;

				if (Utils.IsHexadecimal(token))
					token = long.Parse(token.Substring(2), NumberStyles.HexNumber).ToString();

				Type type = Type.GetType(token, false);
				if (type != null)
					return new Token(TokenType.TypeIdentifier, token, index);

				switch (token)
				{
					case "true":
					case "false":
						return new Token(TokenType.BooleanLiteral, token, index);
					case Syntax.Statements.Return:
						return new Token(TokenType.ReturnStatement, token, index);
					case Syntax.Operators.Or:
						return new Token(TokenType.OrOperator, token, index);
					case Syntax.Operators.BitwiseOR:
						return new Token(TokenType.BitwiseOROperator, token, index);
					case Syntax.Operators.BitwiseAND:
						return new Token(TokenType.BitwiseANDOperator, token, index);
					case Syntax.Comparators.IsEqual:
						return new Token(TokenType.IsEqualOperator, token, index);
					case Syntax.Comparators.IsNotEqual:
						return new Token(TokenType.IsNotEqualOperator, token, index);
					case Syntax.Comparators.IsGreaterThan:
						return new Token(TokenType.IsGreaterThanOperator, token, index);
					case Syntax.Comparators.IsLessThan:
						return new Token(TokenType.IsLessThanOperator, token, index);
					case Syntax.Comparators.IsGreaterThanOrEqual:
						return new Token(TokenType.IsGreaterThanOrEqualOperator, token, index);
					case Syntax.Comparators.IsLessThanOrEqual:
						return new Token(TokenType.IsLessThanOrEqualOperator, token, index);
					case Syntax.Operators.Add:
						return new Token(TokenType.AddOperator, token, index);
					case Syntax.Operators.And:
						return new Token(TokenType.AndOperator, token, index);
					case Syntax.Operators.Subtract:
						return new Token(TokenType.SubtractOperator, token, index);
					case Syntax.Operators.Multiply:
						return new Token(TokenType.MultiplyOperator, token, index);
					case Syntax.Operators.Divide:
						return new Token(TokenType.DivideOperator, token, index);
					case Syntax.Operators.Modulo:
						return new Token(TokenType.ModuloOperator, token, index);
					case Syntax.Operators.Equal:
						return new Token(TokenType.EqualOperator, token, index);
					case "''":
						return new Token(TokenType.CharacterLiteral, "", index); // support empty Characters ('')
					case "{":
						return new Token(TokenType.StartBracket, token, index);
					case "}":
						return new Token(TokenType.EndBracket, token, index);
					case "(":
					case ")":
						return new Token(TokenType.Parenthesis, token, index);
					case Syntax.Conditionals.Else:
						return new Token(TokenType.ElseConditional, token, index);
				}

				if (token[0] == '\"' && token[token.Length - 1] == '\"' && token.OccurrencesOf("\"") == 2)
					return new Token(TokenType.StringLiteral, token.Substring(1, token.Length - 2).FromBase64().Replace("\\\"", "\""), index);
				else if (token[0] == '\'' && token[token.Length - 1] == '\'') // we can't ensure the length here because the contents are encoded
					return new Token(TokenType.CharacterLiteral, token.Substring(1, token.Length - 2).FromBase64(), index);
				else if (token.IndexOf(Syntax.Conditionals.If + "(") == 0 && token[token.Length - 1] == ')')
					return new Token(TokenType.IfConditional, token, index);
				else if (token.IndexOf(Syntax.Conditionals.While + "(") == 0 && token[token.Length - 1] == ')')
					return new Token(TokenType.WhileConditional, token, index);
				else if (token.IndexOf(Syntax.Conditionals.Loop + "(") == 0 && token[token.Length - 1] == ')')
					return new Token(TokenType.LoopConditional, token, index);
				else if (Utils.IsNumber(token))
				{
					try
					{
						if (token.OccurrencesOf(".") == 1)
						{
							Convert.ToDecimal(token);
							return new Token(TokenType.DecimalLiteral, token, index);
						}
						else
						{
							long number = Convert.ToInt64(token);
							if (number <= int.MaxValue && number >= int.MinValue)
							{
								return new Token(TokenType.IntegerLiteral, token, index);
							}
							else
							{
								return new Token(TokenType.Integer64Literal, token, index);
							}
						}
					}
					catch (FormatException)
					{
						throw new BaseException("Number literal is in a invalid format");
					}
					catch (OverflowException)
					{
						throw new BaseException("Number literal is either too large or small to be parsed");
					}
				}
				else if (FunctionChain.IsChain(token))
					return new Token(TokenType.FunctionChain, token, index);
				else if (FunctionBase.IsFunctionCall(token))
					return new Token(TokenType.FunctionCall, token, index);
				else if (Utils.IsValidObjectName(token))
					return new Token(TokenType.VariableIdentifier, token, index);
			}

			if (@throw)
				throw new BaseException("Unknown token: " + token);

			return null;
		}

		private readonly static string[] statements = typeof(Syntax.Statements).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.IsLiteral && !x.IsInitOnly).Select(x => (string)x.GetValue(null)).ToArray();
		private readonly static string[] identifiers = typeof(Syntax.Identifiers).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.IsLiteral && !x.IsInitOnly).Select(x => (string)x.GetValue(null)).ToArray();
		private readonly static string[] operators = typeof(Syntax.Operators).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.IsLiteral && !x.IsInitOnly).Select(x => (string)x.GetValue(null)).ToArray();
		private readonly static string[] comparators = typeof(Syntax.Comparators).GetFields(BindingFlags.Public | BindingFlags.Static).Where(x => x.IsLiteral && !x.IsInitOnly).Select(x => (string)x.GetValue(null)).ToArray();
		private readonly static List<string> terms = Utils.ConcatArrays(statements, identifiers, operators, comparators).ToList();

		public static List<Token> ParseLine(string line, bool @throw = true)
		{
			if (string.IsNullOrWhiteSpace(line))
				return new List<Token>(); // don't parse empty line

			List<Token> tokens = new List<Token>();
			string currentIdentifier = "";
			Token currentToken = new Token();
			for (int i = 0; i < line.Length; i++)
			{
				if ((Utils.IsValidFunctionName(currentIdentifier) || (currentIdentifier.EndsWith(".") && Utils.IsValidObjectName(currentIdentifier.Substring(0, currentIdentifier.Length - 1)))) && line[i] == '(')
				{
					// this is the start of a function, we must handle this differently or it will be parsed into several different tokens
					int closingParenthesis = Utils.GetInitialClosingParenthesis(line.Substring(i));
					if (closingParenthesis == -1)
					{
						throw new BaseException("Function is missing a closing parenthesis");
					}
					else
					{
						// skip to the closingParenthesis, it will be handled below
						int closingParenthesisIndex = i + closingParenthesis;
						currentIdentifier += line.Substring(i, closingParenthesisIndex - i);
						i = closingParenthesisIndex;
					}
				}

				if (currentIdentifier == "")
				{
					foreach (var term in terms)
					{
						if (line.Length > term.Length + i && line.Substring(i, term.Length) == term)
						{
							tokens.Add(ParseToken(term));
							currentIdentifier = "";
							currentToken = new Token();
							i += term.Length;
						}
					}
				}

				if (line[i] == '\"' || line[i] == '\'')
				{
					// this is the start of a string/character literal, we must handle this differently or it will be parsed into several different tokens
					int closing = line.Substring(i + 1).IndexOf(line[i]);
					if (closing == -1)
					{
						if (line[i] == '\"')
							throw new BaseException("String literal is missing a closing quote");
						else
							throw new BaseException("Character literal is missing a closing apostrophe");
					}
					else
					{
						// skip to the closing quote or apostrophe, it will be handled below
						int closeIndex = i + closing + 1;
						currentIdentifier += line.Substring(i, closeIndex - i);
						i = closeIndex;
					}
				}

				if (line[i] == ' ')
				{
					if (currentToken.type != TokenType.None)
					{
						tokens.Add(currentToken);
						currentIdentifier = "";
						currentToken = new Token();
					}
				}
				else
				{
					currentIdentifier += line[i];
					var newToken = ParseToken(currentIdentifier, false);

					if (newToken == null)
					{
						// this letter invalidated the token, add the previous token. Reset the current token, and set the current identifier to the current character
						tokens.Add(currentToken);
						currentIdentifier = line[i].ToString();
						currentToken = ParseToken(currentIdentifier, false);

						if (currentToken == null)
						{
							// current character is not at all valid so we will ignore it
							currentIdentifier = "";
							currentToken = new Token();
						}
					}
					else
					{
						currentToken = newToken;
					}
				}
			}

			if (currentToken.type != TokenType.None)
			{
				tokens.Add(currentToken);
			}

			// now we must link consecutive FunctionCall tokens, and VariableIdentifier and consecutive FunctionCall tokens together to form FunctionChains
			int chainStart = -1;
			for (int i = 0; i < tokens.Count; i++)
			{
				if (chainStart == -1)
				{
					if (tokens[i].type == TokenType.VariableIdentifier || tokens[i].type == TokenType.FunctionCall)
						chainStart = i;
				}
				else if (tokens[i].type != TokenType.FunctionCall)
				{
					if (i - chainStart > 1)
					{
						// chain is longer than 1 token long
						var chainTokens = tokens.GetRange(chainStart, i - chainStart);
						tokens.RemoveRange(chainStart, i - chainStart);

						string identifier = "";
						foreach (var chainToken in chainTokens)
						{
							identifier += chainToken.identifier + ".";
						}
						identifier = identifier.Remove(identifier.Length - 1);

						if (chainTokens.Count == 2 && chainTokens[0].type == TokenType.VariableIdentifier)
						{
							// it's just a method (EX: myVar.ToString())
							tokens.Insert(chainStart, new Token(TokenType.FunctionCall, identifier, chainTokens[0].Index));
						}
						else
						{
							tokens.Insert(chainStart, new Token(TokenType.FunctionChain, identifier, chainTokens[0].Index));
						}

						i = chainStart;
					}

					chainStart = -1;
				}
				else if (i == tokens.Count - 1)
				{
					var chainTokens = tokens.GetRange(chainStart, i - chainStart + 1);
					tokens.RemoveRange(chainStart, i - chainStart + 1);

					string identifier = "";
					foreach (var chainToken in chainTokens)
					{
						identifier += chainToken.identifier + ".";
					}
					identifier = identifier.Remove(identifier.Length - 1);

					if (chainTokens.Count == 2 && chainTokens[0].type == TokenType.VariableIdentifier)
					{
						// it's just a method (EX: myVar.ToString())
						tokens.Insert(chainStart, new Token(TokenType.FunctionCall, identifier, chainTokens[0].Index));
					}
					else
					{
						tokens.Insert(chainStart, new Token(TokenType.FunctionChain, identifier, chainTokens[0].Index));
					}

					break;
				}
			}

			if (@throw && tokens.Count > 1)
			{
				if (tokens[0].type == TokenType.VariableIdentifier && tokens[1].type == TokenType.VariableIdentifier)
					throw new BaseException("Unknown type: " + tokens[0].identifier);
				if (tokens[0].type == TokenType.TypeIdentifier && tokens[1].type == TokenType.TypeIdentifier)
					throw new BaseException("Cannot define a variable with the same name as an existing type");
			}

			return tokens;
		}

		public static string EncodeLiterals(string str)
		{
			// Encode the contents in between "s or 's to base64 so they don't interfere with anything
			MatchCollection literalMatches = Regex.Matches(str, "([\"\'])(?:(?:\\\\\\1|.)*?)\\1"); // matches for text surrounded in "s or 's (non-empty) (keep in mind the matches include the "s or 's)
			int indexOffset = 0;
			foreach (Match match in literalMatches)
			{
				// text inside of the "s or 's
				var inside = match.Value.Substring(1, match.Length - 2);

				if (match.Value[0] == '\'' && match.Length > 3 && inside != "\\\"" && inside != "\\\'")
					throw new BaseException("Character literal must have no more than one character");

				if (match.Length > 2) // not just "s or 's
				{
					string base64 = inside.Replace("\\\"", "\"").Replace("\\'", "\'").ToBase64();
					str = str.Remove(indexOffset + match.Index + 1, match.Length - 2); // remove contents of string
					str = str.Insert(indexOffset + match.Index + 1, base64); // fill contents of string to base64 of old contents
					indexOffset += base64.Length - (match.Length - 2);
				}
			}
			return str;
		}

		public static List<Line> Parse(List<string> codeLines, int currentLineOffset = 0, bool isNestedInFunction = false, bool isNestedInConditional = false)
		{
			Stopwatch sw = new Stopwatch();
			sw.Start();

			List<Line> lines = new List<Line>();
			for (int i = 0; i < codeLines.Count; i++)
			{
				Interpreter.CurrentLine = i + 1 + currentLineOffset;

				while (codeLines[i].IndexOf("	") == 0 || codeLines[i].IndexOf(" ") == 0)
					codeLines[i] = codeLines[i].Remove(0, 1);

				string safeLine = codeLines[i];
				if (!isNestedInConditional && !isNestedInFunction) // literals are already encoded
					safeLine = EncodeLiterals(safeLine);

				while (safeLine.IndexOf(Syntax.Identifiers.Comment) != -1)
				{
					int pos = safeLine.IndexOf(Syntax.Identifiers.Comment);
					safeLine = safeLine.Remove(pos, safeLine.Length - pos);
				} // get rid of all comments and anything after it (but it cannot be enclosed in quotes or apostrophes)

				if (string.IsNullOrWhiteSpace(safeLine))
				{
					lines.Add(new Line(new List<Token>()));
					continue;
				} // no need to parse or evaluate empty line

				lines.Add(new Line(ParseLine(safeLine)));
				Line RecentLine = lines[lines.Count - 1];

				if (!isNestedInFunction && Interpreter.Debug && RecentLine.Count > 0)
				{
					Console.WriteLine("\n(" + Interpreter.CurrentFile + ") Line " + (i + 1) + ": ");
					foreach (var token in RecentLine)
						Console.WriteLine(token.type.ToString() + " - " + token.identifier.Replace("\n", @"\n").Replace(((char)0x1D).ToString(), " "));
				}

				if (Interpreter.Functions.FunctionExists("Define(") && RecentLine.Count > 0 && RecentLine[0].type == TokenType.FunctionCall && RecentLine[0].identifier.IndexOf("Define(") == 0)
				{
					Interpreter.Functions["Define("].Invoke(RecentLine[0].identifier);
					lines[lines.Count - 1] = new Line(new List<Token>()); // line was already handled, clear line
				} // must handle this function before the other lines are compiled to avoid errors
			}

			sw.Stop();
			if (Interpreter.Debug)
				Console.WriteLine("Parsing " + (Interpreter.CurrentFile != "" ? "'" + Interpreter.CurrentFile + "'" : codeLines.Count + " lines") + " took " + sw.ElapsedMilliseconds + " ms");

			return lines;
		}
	}
}