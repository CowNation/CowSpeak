#include "CowConfig.hpp"
#include "Parser.h"
#include <iostream>
#include <vector>
#include <string>
#include <stdlib.h>

/*
* THIS PROJECT HAS BEEN MOSTLY ABANDONED, IT IS CURRENTLY WORKING BUT WILL RECIEVE FEW UPDATES
*/

#define DEBUG 0

int main() {
	std::string filename = "Run.COWFILE";

	CowConfig cfg(filename);
	std::vector< std::string > pLines = cfg.GetLines();
	FileLexer fl(pLines);

	if (DEBUG){
		std::cout << "----------------------------\n";

		for (int i = 0; i < fl.Lines.size(); i++) {
			for (int j = 0; j < fl.Lines[i].type.size(); j++) {
				std::cout << "Line #" << i + 1 << " | Token #" << j + 1 << ": ";
				std::cout << fl.Lines[i].type[j].type << std::endl;
			}
			std::cout << "Line #" << i + 1 << " Executed: " << fl.Lines[i].betaExec(fl.Vars) << std::endl;
			std::cout << "----------------------------\n";
		}
	}
}