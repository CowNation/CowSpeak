#ifndef PARSER
#define PARSER

#include <vector>
#include <string>
#include <iostream>
#include <math.h>
#include "LexedLine.h"

class Parser;
class FileLexer;

class FileLexer {
public:
	std::vector< TokenLine > Lines;
	std::vector< Variable > Vars;
private:
	std::vector < Token > ParseLine(std::string line) {
		if (line.find_first_not_of(' ') == std::string::npos)
			return std::vector< Token >();

		std::vector< std::string > splitLine = SplitString(line, ' ');
		std::vector< Token > ret;
		for (int i = 0; i < splitLine.size(); i++) {
			if (splitLine[i] == printID)
				continue;

			if (splitLine[i].find(")") == splitLine[i].length() - 1 && splitLine[i].find("(") != -1)
				ret.push_back(Token(TokenType::FunctionCall, splitLine[i]));
			else if (splitLine[i] == "-")
				ret.push_back(Token(TokenType::SubtractOperator, splitLine[i]));
			else if (is_digits_only(splitLine[i]))
				ret.push_back(Token(TokenType::Number, splitLine[i]));
			else if (splitLine[i] == "+")
				ret.push_back(Token(TokenType::AddOperator, splitLine[i]));
			else if (splitLine[i] == "*")
				ret.push_back(Token(TokenType::MultiplyOperator, splitLine[i]));
			else if (splitLine[i] == "/")
				ret.push_back(Token(TokenType::DivideOperator, splitLine[i]));
			else if (splitLine[i] == "^")
				ret.push_back(Token(TokenType::PowerOperator, splitLine[i]));
			else if (splitLine[i] == "%")
				ret.push_back(Token(TokenType::ModOperator, splitLine[i]));
			else if (splitLine[i] == "=")
				ret.push_back(Token(TokenType::EqualOperator, splitLine[i]));
			else if (is_letters_only(splitLine[i]))
				ret.push_back(Token(TokenType::VariableIdentifier, splitLine[i]));
			else if (splitLine[i].at(splitLine[i].length()-1) == '#' && splitLine[i].rfind("#", 0) == 0){}
			else {
				FATAL_ERROR("Unknown identifier: " + splitLine[i]);
			}
		}

		return ret;
	}
public:
	FileLexer(std::vector< std::string > fileLines) {
		for (int i = 0; i < fileLines.size(); i++) {
			while (fileLines[i].find("#") != -1){
				int pos = fileLines[i].find("#");
				fileLines[i].erase(pos, fileLines[i].length() - pos);
			} // get rid of all '#' and anything after it

			if (fileLines[i] == "") // no need to parse or evaluate empty line
				continue;

			Lines.push_back(ParseLine(fileLines[i]));

			float retVal = Lines[i].betaExec(Vars);
			if (isIndexValid(0, Lines[i].type) && Lines[i].type[0].type == VariableIdentifier && isIndexValid(1, Lines[i].type) && Lines[i].type[1].type == EqualOperator){
				if (!isVarDefined(Vars, Lines[i].type[0].identifier)) {
					Vars.push_back(Variable(Lines[i].type[0].identifier, 0));
					Vars[Vars.size() - 1].Value = retVal;
				} // create new variable
				else {
					for (int v = 0; v < Vars.size(); v++){
						if (Lines[i].type[0].identifier == Vars[v].Name)
							Vars[v].Value = retVal;
					} // using getVariable does not work for this
				}
			} // first token is VariableIdentifier | second token is EqualOperator

			int printIndex = fileLines[i].find(_printID);
			if (printIndex == 0)
				std::cout << retVal << std::endl; // print executed line
			else if (printIndex != -1)
				FATAL_ERROR("PrintIdentifier must be the first token on a line");
		}
	}
};

#endif // !PARSER
