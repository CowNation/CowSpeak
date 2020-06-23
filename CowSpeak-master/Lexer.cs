using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;
using CowSpeak.Exceptions;

namespace CowSpeak
{
	public static class Lexer
	{
		public static Token ParseToken(string token, bool _throw = true, int Index = -1)
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
					return new Token(TokenType.TypeIdentifier, token, Index);

				switch (token)
				{
					case "true":
					case "false":
						return new Token(TokenType.Boolean, token, Index);
					case Syntax.Statements.Return:
						return new Token(TokenType.ReturnStatement, token, Index);
					case Syntax.Operators.Or:
						return new Token(TokenType.OrOperator, token, Index);
					case Syntax.Operators.BitwiseOR:
						return new Token(TokenType.BitwiseOROperator, token, Index);
					case Syntax.Operators.BitwiseAND:
						return new Token(TokenType.BitwiseANDOperator, token, Index);
					case Syntax.Comparators.IsEqual:
						return new Token(TokenType.IsEqualOperator, token, Index);
					case Syntax.Comparators.IsNotEqual:
						return new Token(TokenType.IsNotEqualOperator, token, Index);
					case Syntax.Comparators.IsGreaterThan:
						return new Token(TokenType.IsGreaterThanOperator, token, Index);
					case Syntax.Comparators.IsLessThan:
						return new Token(TokenType.IsLessThanOperator, token, Index);
					case Syntax.Comparators.IsGreaterThanOrEqual:
						return new Token(TokenType.IsGreaterThanOrEqualOperator, token, Index);
					case Syntax.Comparators.IsLessThanOrEqual:
						return new Token(TokenType.IsLessThanOrEqualOperator, token, Index);
					case Syntax.Operators.Add:
						return new Token(TokenType.AddOperator, token, Index);
					case Syntax.Operators.And:
						return new Token(TokenType.AndOperator, token, Index);
					case Syntax.Operators.Subtract:
						return new Token(TokenType.SubtractOperator, token, Index);
					case Syntax.Operators.Multiply:
						return new Token(TokenType.MultiplyOperator, token, Index);
					case Syntax.Operators.Divide:
						return new Token(TokenType.DivideOperator, token, Index);
					case Syntax.Operators.Modulo:
						return new Token(TokenType.ModuloOperator, token, Index);
					case Syntax.Operators.Equal:
						return new Token(TokenType.EqualOperator, token, Index);
					case "''":
						return new Token(TokenType.Character, "", Index); // support empty Characters ('')
					case "{":
						return new Token(TokenType.StartBracket, token, Index);
					case "}":
						return new Token(TokenType.EndBracket, token, Index);
					case "(":
					case ")":
						return new Token(TokenType.Parenthesis, token, Index);
					case Syntax.Conditionals.Else:
						return new Token(TokenType.ElseConditional, token, Index);
				}

				if (token[0] == '\"' && token[token.Length - 1] == '\"' && token.OccurrencesOf("\"") == 2)
					return new Token(TokenType.String, token.Substring(1, token.Length - 2).FromBase64().Replace("\\\"", "\""), Index);
				else if (token[0] == '\'' && token[token.Length - 1] == '\'') // we can't ensure the length here because the contents are encoded
					return new Token(TokenType.Character, token.Substring(1, token.Length - 2).FromBase64(), Index);
				else if (token.IndexOf(Syntax.Conditionals.If + "(") == 0 && token[token.Length - 1] == ')')
					return new Token(TokenType.IfConditional, token, Index);
				else if (token.IndexOf(Syntax.Conditionals.While + "(") == 0 && token[token.Length - 1] == ')')
					return new Token(TokenType.WhileConditional, token, Index);
				else if (token.IndexOf(Syntax.Conditionals.Loop + "(") == 0 && token[token.Length - 1] == ')')
					return new Token(TokenType.LoopConditional, token, Index);
				else if (Utils.IsNumber(token))
					return new Token(TokenType.Number, token, Index);
				else if (FunctionChain.IsChain(token))
					return new Token(TokenType.FunctionChain, token, Index);
				else if (FunctionBase.IsFunctionCall(token))
					return new Token(TokenType.FunctionCall, token, Index);
				else if (Utils.IsValidObjectName(token))
					return new Token(TokenType.VariableIdentifier, token, Index);
			}

			if (_throw)
				throw new BaseException("Unknown token: " + token);

			return null;
		}

		public static List<Token> ParseLine(string line, bool _throw = true)
		{
			if (string.IsNullOrWhiteSpace(line))
				return new List<Token>(); // don't parse empty line

			string _line = line;
			for (int Occurrence = 0; Occurrence < Utils.OccurrencesOf(line, " "); Occurrence++)
			{
				int i = Utils.OrdinalIndexOf(line, " ", Occurrence);
				char letter = line[i];

				if (letter == ' ' && line.IsIndexBetween(i, '(', ')'))
				{
					char before;
					if (line.Substring(0, i).IndexOf("(") - 1 >= 0)
						before = line[line.Substring(0, i).IndexOf("(") - 1]; // char before the (
					else
						continue;

					if (Regex.IsMatch(before.ToString(), @"\w"))
					{
						StringBuilder fileLine = new StringBuilder(_line);
						fileLine[i] = (char)0x1D;
						_line = fileLine.ToString(); 
					}
				}
			}
			line = _line;

			Tuple<int, string>[] splitLine = Utils.SplitWithIndicies(line, " ");
			List<Token> tokens = new List<Token>();
			for (int i = 0; i < splitLine.Length; i++)
			{
				if (string.IsNullOrWhiteSpace(splitLine[i].Item2))
					continue;

				tokens.Add(ParseToken(splitLine[i].Item2, _throw, splitLine[i].Item1));
			}

			if (_throw && tokens.Count > 1)
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
			MatchCollection LiteralMatches = Regex.Matches(str, "([\"\'])(?:(?:\\\\\\1|.)*?)\\1"); // matches for text surrounded in "s or 's (non-empty) (keep in mind the matches include the "s or 's)
			int IndexOffset = 0;
			foreach (Match match in LiteralMatches)
			{
				if (match.Value[0] == '\'' && match.Length > 3)
					throw new BaseException("Character literal must have no more than one character");

				if (match.Length > 2) // not just "s or 's
				{
					string Base64 = match.Value.Substring(1, match.Value.Length - 2).ToBase64();
					str = str.Remove(IndexOffset + match.Index + 1, match.Length - 2); // remove contents of string
					str = str.Insert(IndexOffset + match.Index + 1, Base64); // fill contents of string to base64 of old contents
					IndexOffset += Base64.Length - (match.Length - 2);
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

				string SafeLine = codeLines[i];
				if (!isNestedInConditional && !isNestedInFunction) // literals are already encoded
					SafeLine = EncodeLiterals(SafeLine);

				while (SafeLine.IndexOf(Syntax.Identifiers.Comment) != -1)
				{
					int pos = SafeLine.IndexOf(Syntax.Identifiers.Comment);
					SafeLine = SafeLine.Remove(pos, SafeLine.Length - pos);
				} // get rid of all comments and anything after it (but it cannot be enclosed in quotes or apostrophes)

				if (string.IsNullOrWhiteSpace(SafeLine))
				{
					lines.Add(new Line(new List<Token>()));
					continue;
				} // no need to parse or evaluate empty line

				lines.Add(new Line(ParseLine(SafeLine)));
				Line RecentLine = lines[lines.Count - 1];

				if (!isNestedInFunction && Interpreter.Debug && RecentLine.Count > 0)
				{
					Console.WriteLine("\n(" + Interpreter.CurrentFile + ") Line " + (i + 1) + ": ");
					foreach (var token in RecentLine)
						Console.WriteLine(token.type.ToString() + " - " + token.identifier.Replace("\n", @"\n").Replace(((char)0x1D).ToString(), " "));
				}

				if (Interpreter.Functions.FunctionExists("Define(") && RecentLine.Count > 0 && RecentLine[0].type == TokenType.FunctionCall && RecentLine[0].identifier.IndexOf("Define(") == 0)
				{
					Interpreter.Functions["Define("].Execute(RecentLine[0].identifier);
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