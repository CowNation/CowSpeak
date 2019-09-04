#include "cpp-CowSpeak-master/CowConfig.hpp"
#include "cpp-CowSpeak-master/Parser.h"
#include <iostream>
#include <vector>
#include <string>
#include <stdlib.h>

/*
* THIS PROJECT HAS BEEN MOSTLY ABANDONED, IT IS CURRENTLY WORKING BUT WILL RECIEVE FEW UPDATES
*/

int main() {
	std::string filename = "Run.COWFILE";

	CowConfig cfg(filename);
	std::vector< std::string > pLines = cfg.GetLines();
	FileLexer fl(pLines);

	std::cout << "----------------------------\n";
	
	for (int i = 0; i < fl.Lines.size(); i++) {
		for (int j = 0; j < fl.Lines[i].type.size(); j++) {
			std::cout << "Line #" << i + 1 << " | Token #" << j + 1 << ": ";
			PrintTokenType(fl.Lines[i].type[j].type);
			std::cout << std::endl;
		}
		std::cout << "Line #" << i + 1 << " Executed: " << fl.Lines[i].Exec(fl.Vars) << std::endl;
		std::cout << "----------------------------\n";
	}
}