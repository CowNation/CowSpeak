using System.Collections.Generic;
using System;
using System.Linq;
using System.IO;

namespace CowSpeak{
	public class FileLexer {
		public List< TokenLine > Lines = new List< TokenLine >();

		private List < Token > ParseLine(string line) {
			if (string.IsNullOrWhiteSpace(line))
				return new List< Token >();

			List< string > splitLine = line.Split(' ').ToList();
			List< Token > ret = new List< Token >();
			for (int i = 0; i < splitLine.Count; i++) {
				if (splitLine[i] == "print")
					continue;

				if (splitLine[i].IndexOf("()") != -1)
					ret.Add(new Token(TokenType.FunctionCall, splitLine[i]));
				else if (splitLine[i] == "(" || splitLine[i] == ")")
					ret.Add(new Token(TokenType.ParenthesesOperator, splitLine[i]));
				else if (splitLine[i] == "-")
					ret.Add(new Token(TokenType.SubtractOperator, splitLine[i]));
				else if (Utils.IsDigitsOnly(splitLine[i]))
					ret.Add(new Token(TokenType.Number, splitLine[i]));
				else if (splitLine[i] == "+")
					ret.Add(new Token(TokenType.AddOperator, splitLine[i]));
				else if (splitLine[i] == "*")
					ret.Add(new Token(TokenType.MultiplyOperator, splitLine[i]));
				else if (splitLine[i] == "/")
					ret.Add(new Token(TokenType.DivideOperator, splitLine[i]));
				else if (splitLine[i] == "^")
					ret.Add(new Token(TokenType.PowerOperator, splitLine[i]));
				else if (splitLine[i] == "%")
					ret.Add(new Token(TokenType.ModOperator, splitLine[i]));
				else if (splitLine[i] == "=")
					ret.Add(new Token(TokenType.EqualOperator, splitLine[i]));
				else if (Utils.IsLettersOnly(splitLine[i]))
					ret.Add(new Token(TokenType.VariableIdentifier, splitLine[i]));
				else if (splitLine[i][splitLine[i].Length-1] == '#' && splitLine[i].LastIndexOf("#", 0) == 0){}
				else {
					Utils.FATAL_ERROR(i + 1,"Unknown identifier: " + splitLine[i]);
				}
			}

			return ret;
		}

		public FileLexer(CowSpeak owner, List< string > fileLines, bool shouldDebug) {
			for (int i = 0; i < fileLines.Count; i++) {
				fileLines[i] = fileLines[i].Replace(@"\n", Environment.NewLine); // \n is not interpreted as a newline in strings

				while (fileLines[i].IndexOf("#") != -1){
					int pos = fileLines[i].IndexOf("#");
					fileLines[i] = fileLines[i].Remove(pos, fileLines[i].Length - pos);
				} // get rid of all '#' and anything after it

				if (fileLines[i] == "") // no need to parse or evaluate empty line
					continue;

				int startIndex = fileLines[i].IndexOf("print '");
				int endIndex = fileLines[i].LastIndexOf("'");
				if (startIndex == 0 && endIndex > startIndex){
					string toPrint = fileLines[i].Substring(startIndex + 7, endIndex - (startIndex + 7));
					Console.Write(toPrint);
					Lines.Add(new TokenLine(new List<Token>())); // prevents bad_alloc exception
					continue;
				} // example line = print 'Hello World'

				int runIndex = fileLines[i].IndexOf("run '");
				if (runIndex == 0){
					runIndex += 5;
					int endQuotes = fileLines[i].LastIndexOf("'");

					if (endQuotes == -1)
						Utils.FATAL_ERROR(i + 1,"Invalid parameter for 'run' keyword");

					string fileName = fileLines[i].Substring(runIndex, endQuotes - runIndex);
					if (File.Exists(fileName))
						owner.Exec(fileName); // Execute file specified
					else
						Utils.FATAL_ERROR(i + 1,fileName + " does not exist");
					Lines.Add(new TokenLine(new List<Token>())); // prevents bad_alloc exception
					continue;
				}


				Lines.Add(new TokenLine(ParseLine(fileLines[i])));

				float retVal = Lines[i].Exec(i, owner.Vars); // Execute line

				if (Utils.isIndexValid(0, Lines[i].tokens) && Lines[i].tokens[0].type == TokenType.VariableIdentifier && Utils.isIndexValid(1, Lines[i].tokens) && Lines[i].tokens[1].type == TokenType.EqualOperator){
					if (!Utils.isVarDefined(owner.Vars, Lines[i].tokens[0].identifier)) {
						owner.Vars.Add(new Variable(Lines[i].tokens[0].identifier, 0));
						owner.Vars[owner.Vars.Count - 1].Value = retVal;
					} // create new variable
					else {
						for (int v = 0; v < owner.Vars.Count; v++){
							if (Lines[i].tokens[0].identifier == owner.Vars[v].Name)
								owner.Vars[v].Value = retVal;
						} // using getVariable does not work for this
					}
				} // first token is VariableIdentifier | second token is EqualOperator

				int printIndex = fileLines[i].IndexOf("print ");
				if (printIndex == 0)
					Console.Write(retVal); // print executed line
				else if (printIndex != -1)
					Utils.FATAL_ERROR(i + 1,"PrintIdentifier must be the first token on a line");
			}
		}
	}
}