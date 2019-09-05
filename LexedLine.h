#ifndef LEXED_LINE
#define LEXED_LINE

#include <vector>
#include <iostream>
#include <string>
#include <math.h>
#include "Utils.h"
#include "InternalFunctions.h"

enum TokenType {
	FunctionCall,
	Number,
	AddOperator,
	SubtractOperator,
	MultiplyOperator,
	DivideOperator,
	PowerOperator,
	ModOperator,
	EqualOperator,
	VariableIdentifier
};

bool IsOperatorToken(TokenType tt){
	return (tt == AddOperator || tt == SubtractOperator || tt == MultiplyOperator || tt == DivideOperator || tt == ModOperator || tt == PowerOperator);
}

void PrintTokenType(TokenType tt){
	if (tt == FunctionCall)
		std::cout << "FunctionCall";
	else if (tt == Number)
		std::cout << "Number";
	else if (tt == AddOperator)
		std::cout << "Add";
	else if (tt == SubtractOperator)
		std::cout << "Subtract";
	else if (tt == MultiplyOperator)
		std::cout << "Multiply";
	else if (tt == DivideOperator)
		std::cout << "Divide";
	else if (tt == EqualOperator)
		std::cout << "Equal";
	else if (tt == VariableIdentifier)
		std::cout << "Variable";
	else if (tt == PowerOperator)
		std::cout << "Power";
	else if (tt == ModOperator)
		std::cout << "Modulo";
	else
		std::cout << "Unknown";		
}

template <class T>
class Function {
	public:
	T (*FuncDef)();
	std::string funcName;
	Function(std::string FunctionName, T (*FunctionDefinition)()) {
		FuncDef = FunctionDefinition;
		funcName = FunctionName;
	}
	bool isVoid(){
		return funcName.find("VOID__") == 0;
	}
};

std::vector< Function< float > > intFX = {
	Function<float>("VOID__exit()", exit),
	Function<float>("VOID__pause()", pause),
	Function<float>("getFifteen()", getFifteen)
};

class Token {
public:
	TokenType type;
	std::string identifier;
	Token(TokenType tt, std::string id){
		type = tt;
		identifier = id;
	}
};

class TokenLine {
private:
	Function<float> findFunction(std::string functionName){
		for (int i = 0; i < intFX.size(); i++){
			if (intFX[i].funcName == functionName)
				return intFX[i];
		}

		FATAL_ERROR("Function " + functionName + " not found");
		exit(-1);
	} // find fuction with matching name
public:
	std::vector< Token > type;

	TokenLine(std::vector< Token > tt){
		type = tt;
	}
	float betaExec(std::vector< Variable > Vars){
		std::vector< Token > toEval = type;

		for (int i = 0; i < type.size(); i++){
			if (type[i].type == EqualOperator){
				toEval = slice(type, i + 1, type.size()-1);
				break;
			}
		} // remove the equal sign and everything to the left

		std::string Evaluated;
		for (int i = 0; i < toEval.size(); i++){
			std::string identifier = toEval[i].identifier;

			if (toEval[i].type == TokenType::VariableIdentifier){
				identifier = std::to_string(getVariable(Vars, identifier).Value);
			} // replace variable name with it's value
			else if (toEval[i].type == TokenType::FunctionCall){
				identifier = std::to_string(findFunction(identifier).FuncDef());
			} // replace function call with it's return value

			Evaluated += identifier;
		}

		float evaluatedValue = 0;
		try{
			evaluatedValue = eval(Evaluated);
		}
		catch (...){
			FATAL_ERROR("Could not evaluate expression '" + Evaluated + "'");
		}

		return evaluatedValue;
	}
};

#endif
