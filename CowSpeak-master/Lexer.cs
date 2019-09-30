using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.IO;

namespace CowSpeak{
	public class FileLexer {
		public List< TokenLine > Lines = new List< TokenLine >();

		private List < Token > ParseLine(string line) {
			if (string.IsNullOrWhiteSpace(line))
				return new List< Token >(); // don't parse empty line

			bool betweenQuotes = false;
			for (int i = 0; i < line.Length; i++){
				if (line[i] == '\"'){
					if (betweenQuotes){
						betweenQuotes = false;
					}
					else{
						betweenQuotes = true;
					}
				}

				if (betweenQuotes && line[i] == ' ')
					line = line.Remove(i, 1).Insert(i, ((char)0x1f).ToString());
			}

			List< string > splitLine = line.Split(' ').ToList();
			List< Token > ret = new List< Token >();
			for (int i = 0; i < splitLine.Count; i++) {
				if (string.IsNullOrWhiteSpace(splitLine[i]))
					continue;

				bool isType = false;
				foreach (VarType type in VarType.GetTypes()){
					if (type.Name == splitLine[i]){
						ret.Add(new Token(TokenType.TypeIdentifier, splitLine[i]));
						isType = true;
						break;
					}
				} // loop through all language defined types and check for a match

				if (isType)
					continue;
				else if (splitLine[i][0] == '\"' && splitLine[i][splitLine[i].Length - 1] == '\"')
					ret.Add(new Token(TokenType.String, splitLine[i].Replace(((char)0x1f).ToString(), " ").Substring(1, splitLine[i].Replace(((char)0x1f).ToString(), " ").Length - 2)));
				else if (splitLine[i][0] == '\'' && splitLine[i][splitLine[i].Length - 1] == '\'' && splitLine[i].Length == 3)
					ret.Add(new Token(TokenType.Character, splitLine[i][splitLine[i].Length - 2].ToString()));
				else if (splitLine[i] == "run")
					ret.Add(new Token(TokenType.RunIdentifier, splitLine[i]));
				else if (splitLine[i] == "print")
					ret.Add(new Token(TokenType.PrintIdentifier, splitLine[i]));
				else if (splitLine[i].IndexOf("()") != -1)
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
					CowSpeak.FATAL_ERROR("Unknown identifier: " + splitLine[i].Replace(((char)0x1f).ToString(), " "));
				}
			}

			return ret;
		}

		public FileLexer(List< string > fileLines, bool shouldDebug) {
			for (int i = 0; i < fileLines.Count; i++) {
				CowSpeak.currentLine = i + 1;

				fileLines[i] = fileLines[i].Replace(@"\n", Environment.NewLine); // \n is not interpreted as a newline in strings

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

				if (Lines[i].tokens.Count >= 2 && Lines[i].tokens[0].type == TokenType.PrintIdentifier){
					Console.Write(retVal.Get().ToString());
				}

				if (Lines[i].tokens.Count >= 2 && Lines[i].tokens[0].type == TokenType.RunIdentifier){
					string currentFile = CowSpeak.currentFile;
					string fileName = retVal.Get().ToString();
					if (File.Exists(fileName))
						CowSpeak.Exec(fileName); // Execute file specified
					else
						CowSpeak.FATAL_ERROR(fileName + " does not exist");
					CowSpeak.currentFile = currentFile; // curr file is not set back after exec of another file
				}

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