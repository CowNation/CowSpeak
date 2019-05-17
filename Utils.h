#ifndef UTILS
#define UTILS

#include <algorithm>
#include <cctype>
#include <functional>
#include <iostream>
#include <stdlib.h>

class Variable {
public:
	std::string varName;
	float Value;
	Variable(std::string VariableName, float VariableValue) {
		varName = VariableName;
		Value = VariableValue;
	}
};

void FATAL_ERROR(std::string errorStr) {
	std::cout << "\nFATAL_ERROR: " << errorStr << std::endl;
	int i;
	std::cin >> i;
	exit(-1);
}

bool isVarDefined(std::vector< Variable > Vars, std::string varName) {
	for (int i = 0; i < Vars.size(); i++) {
		if (Vars[i].varName == varName)
			return true;
	}
	return false;
}
Variable& getNamedVariable(std::vector< Variable > Vars, std::string varName) {
	for (int i = 0; i < Vars.size(); i++) {
		if (Vars[i].varName == varName)
			return Vars[i];
	}
	FATAL_ERROR("Could not find named variable: " + varName);
	exit(0);
}
void assignDefinedVar(std::vector< Variable > Vars, std::string varName, float Val) {
	for (int i = 0; i < Vars.size(); i++) {
		if (Vars[i].varName == varName)
			Vars[i].Value = Val;
			return;
	}
}

bool is_digits_only(const std::string &str)
{
	return str.find_first_not_of(".-0123456789") == std::string::npos && str[str.length()] != '-';
}
bool is_letters_only(std::string str) {
	return std::find_if(str.begin(), str.end(),
		std::not1(std::ptr_fun((int(*)(int))std::isalpha))) == str.end();
}

std::vector< std::string > SplitString(std::string str, char splitter) {
	std::vector< std::string > ret;
	std::string temp;
	for (int i = 0; i < str.length(); i++) {
		if (str[i] == splitter && temp.length() > 0) {
			ret.push_back(temp);
			temp.clear();
		}
		else
			temp += str[i];
	}
	if (temp.length() > 0)
		ret.push_back(temp);
	return ret;
}

#endif