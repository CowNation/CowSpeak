using System;

class MainClass {
	public static void Main (string[] args) {
		CowSpeak.CowSpeak.Exec("main.COWFILE");
	}
}

/*
* Simplified naming convention for static void functions
* Fixed void fuctions being allowed to be used in equations
* Added function to check if a token is an operator
* Added VariableTypes
* Added types: integer (int), decimal (double), string (string), character (char)
* Recoded variable class to support types
* Fixed crash caused by empty lines
* Variable type must be specified when variable is defined
* Added new errors & fixed some old crashes
* Added per line token debugging
* Recoded functionality of the print identifier to work with strings
* Reworked Exec function to return multiple types
* Concatenated 'chains' can be started with a string or char and can include numbers, strings, or characters
* Recoded functionality of the run identifier to work with strings
* Added internal function to deallocate all vars
* Major optimizations
* Made CowSpeak class static
* File program failed in is listed in FATAL_ERROR
* Removed some obsolete Utils
* Static functions can return any type
* Added inputString func
* Replaced void member for functions with the type member
* Throwing errors is optional for getVariable & findFunction
* Variable names are allowed to contain underscores
* Added inputInteger and inputDecimal static function
*/