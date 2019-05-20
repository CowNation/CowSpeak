#ifndef LEXED_LINE
#define LEXED_LINE

#include <vector>
#include <iostream>
#include <string>
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
	template <class c>
	int CheckMatchingFuncName(std::vector< Function< c > > fx, Token tok, int i){
		bool Match = false;
		int retVal = 0;
		for (int j = 0; j < fx.size(); j++){
			if (fx[j].funcName == tok.identifier || "VOID__" + tok.identifier == fx[j].funcName){
				if (fx[j].isVoid() && ((i - 1 > 0 && i + 1 < type.size() && (type[i - 1].type == EqualOperator || type[i + 1].type == EqualOperator)) || IsOperatorToken(type[i - 1].type) || IsOperatorToken(type[i + 1].type))) {
					FATAL_ERROR("Cannot perform operation on void function: " + type[i].identifier);
				}
				retVal = fx[j].FuncDef();
				Match = true;
				break;
			}
		}
		if (!Match)
			FATAL_ERROR("No matching function for call to: " + tok.identifier);
		return retVal;
	}
public:
	std::vector< Token > type;

	TokenLine(std::vector< Token > tt){
		type = tt;
	}
	float Exec(std::vector< Variable > Vars){
		std::vector< std::string > Evaluated;
		float temp = 0;
		bool tempModified = false;
		for (int i = 0; i < type.size(); i++){
			if (IsOperatorToken(type[i].type)){
				Evaluated.push_back(type[i].identifier);
			}
			else if (type[i].type == Number){
				if ((i + 1 < type.size() && type[i + 1].type == Number) || (i - 1 >= 0 && type[i - 1].type == Number)){
					FATAL_ERROR("Missing operator after " + type[i].identifier);
					if ((i + 1 < type.size() && type[i + 1].type == Number))
						std::cout << type[i + 1].identifier << std::endl;
					else
						std::cout << type[i - 1].identifier << std::endl;
					return 0;
				}
				Evaluated.push_back(type[i].identifier);
			}
			else if (type[i].type == EqualOperator) {
				if (i - 1 >= 0 && i + 1 < type.size()) {
					if (type[i - 1].type != Number && type[i - 1].type != VariableIdentifier && type[i - 1].type != FunctionCall)
						FATAL_ERROR("No operands of operator: =");
					if (type[i + 1].type != Number && type[i + 1].type != VariableIdentifier && type[i + 1].type != FunctionCall)
						FATAL_ERROR("No operands of operator: =");
				}
				else
					FATAL_ERROR("No operands of operator: =");
				Evaluated.push_back(type[i].identifier);
			}
			else if (type[i].type == VariableIdentifier) {
				if (isVarDefined(Vars, type[i].identifier)) {
					Evaluated.push_back(std::to_string(getNamedVariable(Vars, type[i].identifier).Value));
				}
				else {
					FATAL_ERROR("Unknown VariableIdentifier: " + type[i].identifier);
				}
			}
			else if (type[i].type == FunctionCall){
				Evaluated.push_back(std::to_string(CheckMatchingFuncName<float>(intFX, type[i], i)));
			}
		}
		if (Evaluated.size() == 1) 
			return std::stof(Evaluated[0]);
		if (Evaluated.size() == 3 && is_digits_only(Evaluated[2]) && Evaluated[1] == "=")
			return std::stof(Evaluated[2]);
		//for (int i = 0; i < Evaluated.size(); i++)
		//	std::cout << Evaluated[i] << " ";
		std::cout << std::endl;
		for (int i = 0; i < Evaluated.size(); i++){
			if (IsOperatorToken(type[i].type)){
				if (i - 1 >= 0 && i + 1 < Evaluated.size()){
					if (type[i - 1].type != Number && type[i - 1].type != VariableIdentifier && type[i - 1].type != FunctionCall)
						FATAL_ERROR("No operands of operator: " + type[i].identifier);
					if (type[i + 1].type != Number && type[i + 1].type != VariableIdentifier && type[i + 1].type != FunctionCall)
						FATAL_ERROR("No operands of operator: " + type[i].identifier);
					
					float left;
					float right;

					if (Evaluated[i - 1].find(".") == -1)
						left = std::stoi(Evaluated[i - 1]);
					else
						left = std::stof(Evaluated[i - 1]);

					if (Evaluated[i + 1].find(".") == -1)
						right = std::stoi(Evaluated[i + 1]);
					else
						right = std::stof(Evaluated[i + 1]);

					if (tempModified)
						left = temp;

					if (type[i].type == AddOperator)
						temp = left + right;
					else if (type[i].type == SubtractOperator)
						temp = left - right;
					else if (type[i].type == MultiplyOperator)
						temp = left * right;
					else if (type[i].type == DivideOperator)
						temp = left / right;
					else if (type[i].type == PowerOperator)
						temp = pow(left, right);
					else if (type[i].type == ModOperator)
						temp = left % right;
					tempModified = true;
				}
				else{
					FATAL_ERROR("No operands of operator: " + type[i].identifier);
				}
			}
		}
		return temp;
	}
};

#endif
