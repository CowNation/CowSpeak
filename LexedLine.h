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
	VariableIdentifier,
	ParenthesesOperator
};

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
