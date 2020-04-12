using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CowSpeak
{
	public class Lexer
	{
		internal List< Line > Lines = new List< Line >();

		public static Token ParseToken(string token, bool _throw = true)
		{
			foreach (Definition Definition in CowSpeak.Definitions)
				if (token == Definition.from)
					token = Definition.to;

			if (Utils.IsHexadecimal(token))
				token = int.Parse(token.Substring(2), System.Globalization.NumberStyles.HexNumber).ToString(); // determine if it's a hexadecimal number

			Type type = Utils.GetType(token, false);
			if (type != null)
				return new Token(TokenType.TypeIdentifier, token);

			switch (token)
			{
				case "True":
				case "False":
					return new Token(TokenType.Boolean, token);
				case Syntax.Statements.Return:
					return new Token(TokenType.ReturnStatement, token);
				case Syntax.Comparators.IsEqual:
					return new Token(TokenType.IsEqualOperator, token);
				case Syntax.Comparators.IsNotEqual:
					return new Token(TokenType.IsNotEqualOperator, token);
				case Syntax.Comparators.IsGreaterThan:
					return new Token(TokenType.IsGreaterThanOperator, token);
				case Syntax.Comparators.IsLessThan:
					return new Token(TokenType.IsLessThanOperator, token);
				case Syntax.Comparators.IsGreaterThanOrEqual:
					return new Token(TokenType.IsGreaterThanOrEqualOperator, token);
				case Syntax.Comparators.IsLessThanOrEqual:
					return new Token(TokenType.IsLessThanOrEqualOperator, token);
				case Syntax.Operators.Add:
					return new Token(TokenType.AddOperator, token);
				case Syntax.Operators.And:
					return new Token(TokenType.AndOperator, token);
				case Syntax.Operators.Subtract:
					return new Token(TokenType.SubtractOperator, token);
				case Syntax.Operators.Multiply:
					return new Token(TokenType.MultiplyOperator, token);
				case Syntax.Operators.Divide:
					return new Token(TokenType.DivideOperator, token);
				case Syntax.Operators.Modulo:
					return new Token(TokenType.ModuloOperator, token);
				case Syntax.Operators.Equal:
					return new Token(TokenType.EqualOperator, token);
				case "''":
					return new Token(TokenType.Character, ""); // support empty Characters ('')
				case "{":
					return new Token(TokenType.StartBracket, token);
				case "}":
					return new Token(TokenType.EndBracket, token);
				case "(":
				case ")":
					return new Token(TokenType.Parenthesis, token);
				case Syntax.Conditionals.Else:
					return new Token(TokenType.ElseConditional, token);
			}

			if (token[0] == '\"' && token[token.Length - 1] == '\"' && token.OccurrencesOf("\"") == 2)
				return new Token(TokenType.String, token.Substring(1, token.Length - 2).FromBase64());	
			else if (token[0] == '\'' && token[token.Length - 1] == '\'') // we can't ensure the length here because the contents are encoded
				return new Token(TokenType.Character, token.Substring(1, token.Length - 2).FromBase64());
			else if (token.IndexOf(Syntax.Conditionals.If + "(") == 0 && token[token.Length - 1] == ')')
				return new Token(TokenType.IfConditional, token);
			else if (token.IndexOf(Syntax.Conditionals.While + "(") == 0 && token[token.Length - 1] == ')')
				return new Token(TokenType.WhileConditional, token);
			else if (token.IndexOf(Syntax.Conditionals.Loop + "(") == 0 && token[token.Length - 1] == ')')
				return new Token(TokenType.LoopConditional, token);
			else if (Utils.IsNumber(token))
				return new Token(TokenType.Number, token);
			else if (FunctionChain.IsChain(token))
				return new Token(TokenType.FunctionChain, token);
			else if (FunctionBase.IsFunctionCall(token))
				return new Token(TokenType.FunctionCall, token);
			else if (Utils.IsValidObjectName(token))
				return new Token(TokenType.VariableIdentifier, token);

			if (_throw)
				throw new Exception("Unknown token: " + token);

			return null;
		}

		public static List < Token > ParseLine(string line, bool _throw = true)
		{
			if (string.IsNullOrWhiteSpace(line))
				return new List< Token >(); // don't parse empty line

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

					if (Regex.IsMatch(before.ToString(), "\\w"))
					{
						StringBuilder fileLine = new StringBuilder(_line);
						fileLine[i] = (char)0x1D;
						_line = fileLine.ToString(); 
					}
				}
			}
			line = _line;

			List< string > splitLine = line.Split(' ').ToList();
			List< Token > ret = new List< Token >();
			List< int > skipList = new List< int >(); // lines to be skipped
			for (int i = 0; i < splitLine.Count; i++)
			{
				if (string.IsNullOrWhiteSpace(splitLine[i]))
					continue;

				foreach (int skip in skipList)
					if (skip == i)
						continue;

				ret.Add(ParseToken(splitLine[i], _throw));
			}

			return ret;
		}

		internal Lexer()
		{

		}

		public static string EncodeLiterals(string str)
		{
			// Encode the contents in between "s or 's to base64 so they don't interfere with anything
			MatchCollection LiteralMatches = Regex.Matches(str, "([\"\'])(?:(?:\\\\\\1|.)*?)\\1"); // matches for text surrounded in "s or 's (non-empty) (keep in mind the matches include the "s or 's)
			int IndexOffset = 0;
			foreach (Match match in LiteralMatches)
			{
				if (match.Value[0] == '\'' && match.Length > 3)
					throw new Exception("Character literal must have no more than one character");

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

		internal void Tokenize(List< string > fileLines, int CurrentLineOffset = 0, bool isNestedInFunction = false, bool isNestedInConditional = false, FileType Type = FileType.Normal)
		{
			if (Type == FileType.Binary)
			{
				for (int i = 0; i < fileLines.Count; i++)
				{
					CowSpeak.CurrentLine = i + 1 + CurrentLineOffset;
					string Built = "";

					try
					{
						fileLines[i].Split(' ').ToList().ForEach(x => Built += Encoding.ASCII.GetString(Utils.GetBytesFromBinaryString(x)));
					}
					catch
					{
						throw new Exception("Invalid binary token in bcf");
					}
					fileLines[i] = Built;
				}
			}
			else if (Type == FileType.Hex)
			{
				for (int i = 0; i < fileLines.Count; i++)
				{
					CowSpeak.CurrentLine = i + 1 + CurrentLineOffset;
					string Built = "";

					try
					{
						fileLines[i].Split(' ').Where(x => x != "").ToList().ForEach(x => Built += (char)int.Parse(x, NumberStyles.HexNumber));
					}
					catch (System.FormatException)
					{
						throw new Exception("Invalid hexadecimal token in hcf");
					}

					fileLines[i] = Built;
				}
			}

			for (int i = 0; i < fileLines.Count; i++)
			{
				CowSpeak.CurrentLine = i + 1 + CurrentLineOffset;

				while (fileLines[i].IndexOf("	") == 0 || fileLines[i].IndexOf(" ") == 0)
					fileLines[i] = fileLines[i].Remove(0, 1);

				string SafeLine = fileLines[i];
				if (!isNestedInConditional && !isNestedInFunction) // literals are already encoded
					SafeLine = EncodeLiterals(SafeLine);

				while (SafeLine.IndexOf(Syntax.Identifiers.Comment) != -1)
				{
					int pos = SafeLine.IndexOf(Syntax.Identifiers.Comment);
					SafeLine = SafeLine.Remove(pos, SafeLine.Length - pos);
				} // get rid of all Comments and anything after it (but it cannot be enclosed in quotes or apostrophes)

				if (string.IsNullOrWhiteSpace(SafeLine))
				{
					Lines.Add(new Line(new List<Token>()));
					continue;
				} // no need to parse or evaluate empty line

				Lines.Add(new Line(ParseLine(SafeLine)));
				Line RecentLine = Lines[Lines.Count - 1];

				if (RecentLine.Count >= 2 && RecentLine[0].type == TokenType.VariableIdentifier && RecentLine[1].type == TokenType.VariableIdentifier)
					throw new Exception("Unknown token: " + RecentLine[0].identifier);

				if (!isNestedInFunction && CowSpeak.Debug && RecentLine.Count > 0)
				{
					System.Console.WriteLine("\n(" + CowSpeak.CurrentFile + ") Line " + (i + 1) + ": ");
					foreach (var token in RecentLine)
						System.Console.WriteLine(token.type.ToString() + " - " + token.identifier.Replace(System.Environment.NewLine, @"\n").Replace(((char)0x1D).ToString(), " "));
				}

				if (RecentLine.Count > 0 && RecentLine[0].type == TokenType.FunctionCall && RecentLine[0].identifier.IndexOf("Define(") == 0)
				{
					CowSpeak.Functions.Get("Define(").Execute(RecentLine[0].identifier);
					Lines[Lines.Count - 1] = new Line(new List<Token>()); // line was already handled, clear line
				} // must handle this function before the other lines are compiled to avoid errors
			}

			CowSpeak.Debug = false; // only debug tokens on compilation, needed because many things recurse back to Lexer

			Executor.Execute(Lines, CurrentLineOffset, isNestedInFunction, isNestedInConditional);
		}
	}
}