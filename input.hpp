#include <termios.h>
#include <unistd.h>
#include <iostream>
#include <vector>
#include <string>

char getch(void)
{
	char buf = 0;
	struct termios old = {0};
	fflush(stdout);
	if(tcgetattr(0, &old) < 0)
		perror("tcsetattr()");
	old.c_lflag &= ~ICANON;
	old.c_lflag &= ~ECHO;
	old.c_cc[VMIN] = 1;
	old.c_cc[VTIME] = 0;
	if(tcsetattr(0, TCSANOW, &old) < 0)
		perror("tcsetattr ICANON");
	if(read(0, &buf, 1) < 0)
		perror("read()");
	old.c_lflag |= ICANON;
	old.c_lflag |= ECHO;
	if(tcsetattr(0, TCSADRAIN, &old) < 0)
		perror("tcsetattr ~ICANON");
	return buf;
}

class Keyboard{
private:
	char keyPressed;
public:
	std::vector< char > allowedKeys; // if there are no allowedKeys, all keys will be allowed
	std::vector< char > blackListedKeys;

	Keyboard(){}

	bool isKeyAllowed(char key){
		if (allowedKeys.size() == 0){
			for (int i = 0; i < blackListedKeys.size(); i++){
				if (blackListedKeys[i] == key)
					return false;
			}

			return true;
		}

		for (int i = 0; i < allowedKeys.size(); i++){
			if (allowedKeys[i] == key){
				for (int p = 0; p < blackListedKeys.size(); p++){
					if (blackListedKeys[p] == key)
						return false;
				}

				return true;
			}
		}

		return false;
	}

	void Update(){
		char tempKeyPressed = getch();
		if (isKeyAllowed(tempKeyPressed))
			keyPressed = tempKeyPressed;
		else
			keyPressed = '\0';
	}

	char GetPressedKey(){
		return keyPressed;
	}

	void WhiteListKeys(std::vector< char > whiteListedKeys){
		for (int j = 0; j < whiteListedKeys.size(); j++)
			allowedKeys.push_back(whiteListedKeys[j]);
	}

	void BlackListKeys(std::vector< char > _blackListedKeys){
		for (int i = 0; i < _blackListedKeys.size(); i++)
			blackListedKeys.push_back(_blackListedKeys[i]);
	}
};