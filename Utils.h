#ifndef UTILS
#define UTILS

#include <algorithm>
#include <cctype>
#include <functional>
#include <iostream>
#include <stdlib.h>
#include <string.h>

#define printID "print"
#define _printID "print "

using namespace std;

template<typename T>
std::vector<T> slice(std::vector<T> const &v, int m, int n)
{
    auto first = v.cbegin() + m;
    auto last = v.cbegin() + n + 1;

    std::vector<T> vec(first, last);
    return vec;
}

double eval(string expr)
{
    string xxx; // Get Rid of Spaces
    for (int i = 0; i < expr.length(); i++)
    {
        if (expr[i] != ' ')
        {
            xxx += expr[i];
        }
    }

    string tok = ""; // Do parantheses first
    for (int i = 0; i < xxx.length(); i++)
    {
        if (xxx[i] == '(')
        {
            int iter = 1;
            string token;
            i++;
            while (true)
            {
                if (xxx[i] == '(')
                {
                    iter++;
                } else if (xxx[i] == ')')
                {
                    iter--;
                    if (iter == 0)
                    {
                        i++;
                        break;
                    }
                }
                token += xxx[i];
                i++;
            }
            //cout << "(" << token << ")" << " == " << to_string(eval(token)) <<  endl;
            tok += to_string(eval(token));
        }
        tok += xxx[i];
    }

    for (int i = 0; i < tok.length(); i++)
    {
        if (tok[i] == '+')
        {
            //cout << tok.substr(0, i) + " + " +  tok.substr(i+1, tok.length()-i-1) << " == " << eval(tok.substr(0, i)) + eval(tok.substr(i+1, tok.length()-i-1)) << endl;
            return eval(tok.substr(0, i)) + eval(tok.substr(i+1, tok.length()-i-1));
        } else if (tok[i] == '-')
        {
            //cout << tok.substr(0, i) + " - " +  tok.substr(i+1, tok.length()-i-1) << " == " << eval(tok.substr(0, i)) - eval(tok.substr(i+1, tok.length()-i-1)) << endl;
            return eval(tok.substr(0, i)) - eval(tok.substr(i+1, tok.length()-i-1));
        }
    }

    for (int i = 0; i < tok.length(); i++)
    {
        if (tok[i] == '*')
        {
            //cout << tok.substr(0, i) + " * " +  tok.substr(i+1, tok.length()-i-1) << " == " << eval(tok.substr(0, i)) * eval(tok.substr(i+1, tok.length()-i-1)) << endl;
            return eval(tok.substr(0, i)) * eval(tok.substr(i+1, tok.length()-i-1));
        } else if (tok[i] == '/')
        {
            //cout << tok.substr(0, i) + " / " +  tok.substr(i+1, tok.length()-i-1) << " == " << eval(tok.substr(0, i)) / eval(tok.substr(i+1, tok.length()-i-1)) << endl;
            return eval(tok.substr(0, i)) / eval(tok.substr(i+1, tok.length()-i-1));
        } else if (tok[i] == '%')
		{
			return (int)eval(tok.substr(0, i)) % (int)eval(tok.substr(i+1, tok.length()-i-1)); // modulus with doubles are illegal
		}
    }

    //cout << stod(tok.c_str()) << endl;
    return stod(tok.c_str()); // Return the value...
}

template <class T>
bool isIndexValid(int index, std::vector< T > container){
	return index >= 0 && index < container.size() && container.size() > 0;
}

bool isDecimal(std::string input){
	return input.find(".") != -1;
}

float varToType(std::string input){
	if (!isDecimal(input)) // find decimal point to determine type
		return std::stoi(input); // type is int
	else
		return std::stof(input); // type is float
}

std::string replaceChar(std::string str, char ch1, char ch2) {
  for (int i = 0; i < str.length(); ++i) {
    if (str[i] == ch1)
      str[i] = ch2;
  }

  return str;
}

class Variable {
public:
	std::string Name;
	float Value;
	Variable(std::string VariableName, float VariableValue) {
		Name = VariableName;
		Value = VariableValue;
	}
};

void FATAL_ERROR(std::string errorStr) {
	std::cout << "\nFATAL_ERROR: " << errorStr << std::endl;
	system("pause");
	exit(-1);
}

bool isVarDefined(std::vector< Variable > Vars, std::string varName) {
	for (int i = 0; i < Vars.size(); i++) {
		if (Vars[i].Name == varName)
			return true;
	}
	return false;
}

Variable& getVariable(std::vector< Variable > Vars, std::string varName) {
	for (int i = 0; i < Vars.size(); i++) {
		if (Vars[i].Name == varName)
			return Vars[i];
	}
	FATAL_ERROR("Could not find variable: " + varName);
	exit(-1);
}

void assignDefinedVar(std::vector< Variable > Vars, std::string varName, float Val) {
	for (int i = 0; i < Vars.size(); i++) {
		if (Vars[i].Name == varName){
			Vars[i].Value = Val;
			return;
		}
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
