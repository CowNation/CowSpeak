using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace CowSpeak
{
	public class Lexer
	{
		public List< Line > Lines = new List< Line >();

		public static Token ParseToken(string token, bool _throw = true)
		{
			try
			{
				foreach (Definition Definition in CowSpeak.Definitions)
					if (token == Definition.from)
						token = Definition.to;
			}
			catch (System.NullReferenceException)
			{

			} // CowSpeak has not been initialized yet, usually means that this func has been called while getting static funcs

			token = Utils.FixBoolean(token);

			if (Utils.IsHexadecimal(token))
				token = int.Parse(token.Substring(2), System.Globalization.NumberStyles.HexNumber).ToString(); // determine if it's a hexadecimal number

			Type type = Utils.GetType(token, false);
			if (type != null)
				return new Token(TokenType.TypeIdentifier, token);

			if (token[0] == '\"' && token[token.Length - 1] == '\"')
				return new Token(TokenType.String, token.Replace(((char)0x1f).ToString(), " ").Substring(1, token.Replace(((char)0x1f).ToString(), " ").Length - 2));
			else if (token == "''")
				return new Token(TokenType.Character, ""); // support empty Characters ('')
			else if (token[0] == '\'' && token[token.Length - 1] == '\'' && token.Length == 3)
				return new Token(TokenType.Character, token[token.Length - 2].ToString());
			else if (token == "}")
				return new Token(TokenType.EndBracket, token);
			else if (token.IndexOf(Syntax.Conditionals.If + "(") == 0 && token[token.Length - 1] == '{' && token[token.Length - 2] == ')')
				return new Token(TokenType.IfConditional, token);
			else if (token.IndexOf(Syntax.Conditionals.Else) == 0 && token[token.Length - 1] == '{')
				return new Token(TokenType.ElseConditional, token);
			else if (token.IndexOf(Syntax.Conditionals.While + "(") == 0 && token[token.Length - 1] == '{' && token[token.Length - 2] == ')')
				return new Token(TokenType.WhileConditional, token);
			else if (token.IndexOf(Syntax.Conditionals.Loop + "(") == 0 && token[token.Length - 1] == '{' && token[token.Length - 2] == ')')
				return new Token(TokenType.LoopConditional, token);
			else if (token == Syntax.Statements.Delete)
				return new Token(TokenType.DeleteIdentifier, token);
			else if (Utils.IsDigitsOnly(token))
				return new Token(TokenType.Number, token);
			else if (token == Syntax.Statements.Return)
				return new Token(TokenType.ReturnStatement, token);
			else if (token == Syntax.Comparators.IsEqual)
				return new Token(TokenType.IsEqualOperator, token);
			else if (token == Syntax.Comparators.IsNotEqual)
				return new Token(TokenType.IsNotEqualOperator, token);
			else if (token == Syntax.Comparators.IsGreaterThan)
				return new Token(TokenType.IsGreaterThanOperator, token);
			else if (token == Syntax.Comparators.IsLessThan)
				return new Token(TokenType.IsLessThanOperator, token);
			else if (token == Syntax.Comparators.IsGreaterThanOrEqual)
				return new Token(TokenType.IsGreaterThanOrEqualOperator, token);
			else if (token == Syntax.Comparators.IsLessThanOrEqual)
				return new Token(TokenType.IsLessThanOrEqualOperator, token);
			else if (token == Syntax.Operators.Add)
				return new Token(TokenType.AddOperator, token);
			else if (token == Syntax.Operators.And)
				return new Token(TokenType.AndOperator, token);
			else if (token == Syntax.Operators.Subtract)
				return new Token(TokenType.SubtractOperator, token);
			else if (token == Syntax.Operators.Multiply)
				return new Token(TokenType.MultiplyOperator, token);
			else if (token == Syntax.Operators.Divide)
				return new Token(TokenType.DivideOperator, token);
			else if (token == Syntax.Operators.Power)
				return new Token(TokenType.PowerOperator, token);
			else if (token == Syntax.Operators.Modulo)
				return new Token(TokenType.ModuloOperator, token);
			else if (token == Syntax.Operators.Equal)
				return new Token(TokenType.EqualOperator, token);
			else if (token.IndexOf("(") != -1 && token[token.Length - 1] == ')')
				return new Token(TokenType.FunctionCall, token);
			else if (token.IndexOf("(") != -1 && token[token.Length - 2] == ')' && token[token.Length - 1] == '{')
				return new Token(TokenType.FunctionDefinition, token);
			else if (Utils.IsLettersOnly(token))
				return new Token(TokenType.VariableIdentifier, token);

			if (_throw)
				throw new Exception("Unknown token: " + token.Replace(((char)0x1f).ToString(), " "));

			return null;
		}

		public static List < Token > ParseLine(string line, bool _throw = true)
		{
			if (string.IsNullOrWhiteSpace(line))
				return new List< Token >(); // don't parse empty line

			line = Utils.ReplaceBetween(line, ' ', '\"', '\"', (char)0x1f);
			line = Utils.ReplaceBetween(line, ' ', '(', ')', (char)0x1D);

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

		public Lexer(List< string > fileLines, int CurrentLineOffset = 0, bool isNestedInFunction = false, bool isNestedInConditional = false)
		{
			for (int i = 0; i < fileLines.Count; i++)
			{
				CowSpeak.CurrentLine = i + 1 + CurrentLineOffset;

				fileLines[i] = Utils.FixBoolean(fileLines[i].Replace(@"\n", System.Environment.NewLine)); // interpret \n as a newline in strings & support for setting booleans using true and false

				while (fileLines[i].IndexOf("	") == 0 || fileLines[i].IndexOf(" ") == 0)
					fileLines[i] = fileLines[i].Remove(0, 1);

				while (fileLines[i].IndexOf(Syntax.Identifiers.Comment) != -1)
				{
					int pos = fileLines[i].IndexOf(Syntax.Identifiers.Comment);
					if (Utils.IsBetween(fileLines[i], pos, '"', '"') || Utils.IsBetween(fileLines[i], pos, '\'', '\''))
					{
						StringBuilder fileLine = new StringBuilder(fileLines[i]);
						fileLine[pos] = (char)0x1f;
						fileLines[i] = fileLine.ToString(); 
						continue;
						// replace with placeholder temporarily to prevent while loop forever
					}

					fileLines[i] = fileLines[i].Remove(pos, fileLines[i].Length - pos);
				} // get rid of all Comments and anything after it (but it cannot be enclosed in quotes or apostrophes)

				fileLines[i] = fileLines[i].Replace(((char)0x1f).ToString(), Syntax.Identifiers.Comment); // replace placeholders back with comment token

				if (string.IsNullOrWhiteSpace(fileLines[i]))
				{
					Lines.Add(new Line(new List<Token>()));
					continue;
				} // no need to parse or evaluate empty line

				Lines.Add(new Line(ParseLine(fileLines[i])));
				Line recentLine = Lines[Lines.Count - 1];

				if (!isNestedInFunction && CowSpeak.Debug && recentLine.Count > 0)
				{
					System.Console.WriteLine("\n(" + CowSpeak.CurrentFile + ") Line " + (i + 1) + ": ");
					foreach (var token in recentLine)
						System.Console.WriteLine(token.type.ToString() + " - " + token.identifier.Replace(System.Environment.NewLine, @"\n").Replace(((char)0x1f).ToString(), " ").Replace(((char)0x1D).ToString(), " "));
				}

				if (recentLine.Count > 0 && recentLine[0].type == TokenType.FunctionCall && recentLine[0].identifier.IndexOf("Define(") == 0)
				{
					CowSpeak.GetFunction("Define(").Execute(recentLine[0].identifier);
					Lines[Lines.Count - 1] = new Line(new List<Token>()); // line was already handled, clear line
				} // must handle this function before the other lines are compiled to avoid errors
			}

			CowSpeak.Debug = false; // only debug tokens on compilation, needed because many things recurse back to Lexer

			Executor.Execute(Lines, fileLines, CurrentLineOffset, isNestedInFunction, isNestedInConditional);
		}
	}
}