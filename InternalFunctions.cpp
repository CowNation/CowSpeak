#include "InternalFunctions.h"
#include <stdlib.h>
#include "input.hpp"

float getFifteen(){
	return 15;
} 

float VOID_pause() {
	system("pause");
	return 0;
}

float VOID_exit() {
	exit(-1);
}

float getInput(){
	Keyboard kb;
	kb.WhiteListKeys({'1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '.', 127, 10}); // 127 is the Decimal code for DEL
	std::string constructed;
	char pressedKey;

	do{
		kb.Update();
		pressedKey = kb.GetPressedKey();

		if (pressedKey == 127 && constructed.length() > 0){
			constructed.pop_back();
			std::cout << "\b";
		}
		else if (pressedKey != 0){
			constructed += pressedKey;
			std::cout << pressedKey;
		}

	} while (pressedKey != 10);
	return std::stof(constructed);
}

float VOID_clrConsole(){
	std::cout << "\033[2J\033[0;0H";
	return 0;
}