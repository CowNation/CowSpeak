#include "CowConfig.hpp"
#include "Parser.h"
#include <iostream>
#include <vector>
#include <string>

#define DEBUG 1

int main() {
	std::string filename = "Run.COWFILE";

	CowConfig cfg(filename);
	std::vector< std::string > pLines = cfg.GetLines();
	FileLexer fl(pLines);
}