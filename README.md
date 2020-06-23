
[![version](https://img.shields.io/badge/version-8-blue)](https://github.com/CowNation/CowSpeak/releases/tag/v8)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)
[![Run on Repl.it](https://repl.it/badge/github/CowNation/CowSpeak)](https://repl.it/github/CowNation/CowSpeak)
# CowSpeak
Ever wish for an interpreted programming language with a C style syntax? Here's your solution! CowSpeak is an open source interpreted programming language made in C# with a C style syntax
## Info
This project's sole developer is a high school student and this is simply a hobby. This language uses CowConfig and isn't thread safe. To see CowConfig go to https://github.com/CowNation/CowConfig
## Features
* Data types: object, boolean, integer, integer64, character, string, and decimal
* Arrays
* Most standard C# operators (+, -, *, /, =, %, =, &&, ||, &, |)
* Single line comments
* Conditionals (if, else, loop, while)
* Custom C# modules
* Language defined functions
* OS specific API functions
* User defined functions
* Methods
* Detailed error throwing
* Simple per-line token debugging
* Scopes & manual variable deletion
* Easily modifiable syntax (even at runtime)
* Full support for hexadecimal numbers
## Modules
### [Main](https://github.com/CowNation/CowSpeak/blob/master/CowSpeak-master/Modules/Main.cs)
* [Generated Documentation](https://github.com/CowNation/CowSpeak/blob/master/CowSpeak-master/Modules/Main.md)
* This module contains standard CowSpeak functions that should work on all platforms
### [Windows](https://github.com/CowNation/CowSpeak/blob/master/CowSpeak-master/Modules/Windows.cs)
* [Generated Documentation](https://github.com/CowNation/CowSpeak/blob/master/CowSpeak-master/Modules/Windows.md)
* This contains various API functions and predefined Windows enums as definitions.
* This will be loaded automatically on first call to CowSpeak.Execute if you are using CowSpeak on a Windows PC and CowSpeak.UseOSSpecificModules is true.
### [Linux](https://github.com/CowNation/CowSpeak/blob/master/CowSpeak-master/Modules/Linux.cs)
* This is simply a placeholder module with no definitions or functions because I don't use linux
* Contributions to this would be greatly appreciated
### [Shorter Type Names](https://github.com/CowNation/CowSpeak/blob/master/CowSpeak-master/Modules/ShorterTypeNames.cs)
* [Generated Documentation](https://github.com/CowNation/CowSpeak/blob/master/CowSpeak-master/Modules/ShorterTypeNames.md)
* This module is for those who dislike how CowSpeak's defult type names are so long
* The existing CowSpeak type names will still work as well
* Function names will still refer to the types as their defualt CowSpeak type names (Ex: myInt.ToCharacter())
* Exceptions will still also refer to the standard CowSpeak type names
* This will probably cause several bugs whenever using this module's short type names
## Significant Versions
[v1 (Initial release on May 17, 2019)](https://github.com/CowNation/CowSpeak/tree/295d57e0a54622b5fc0483c6d1f163408d728aaf)

[v2 (Recode of line evaluations on September 5, 2019)](https://github.com/CowNation/CowSpeak/tree/75c0002235ae917f6d7070cbc35dbfa2c4bb56a8)

[v3 (Recoded in C# on September 25, 2019)](https://github.com/CowNation/CowSpeak/tree/dc7ad0acd7648f64796d9b953425475d3b484e84)

[v4 (Added support for different variable types on September 30, 2019)](https://github.com/CowNation/CowSpeak/tree/90227f3c37685d1286094b6b637fd45f392e4ff5)

[v5 (Added support for parameters and conditional statements on October 8, 2019)](https://github.com/CowNation/CowSpeak/tree/72e3cfb9407a0c6485eb1945b61467331320e43f)

[v6 (Added support for user defined functions and recoded function system on November 20, 2019)](https://github.com/CowNation/CowSpeak/tree/b6c29a7e948dfcfc52dbf721a62bf82a8de469c1)

[v6.5 (Added support for FunctionChains and major bug fixes and improvements February 24, 2020)](https://github.com/CowNation/CowSpeak/tree/6b94fee059b53e8fea5a3d3efa2a0c5ad34b1b86)

[v7 (Added arrays, rewrote StaticFunctions, major bug fixes, and major improvements March 6, 2020)](https://github.com/CowNation/CowSpeak/tree/84b3b0d5186592d87e96f47a9dc55a744850e70a)

[v7.2 (Major bug fixes, improvements, and changed some literals to be stored as Base64 April 12, 2020)](https://github.com/CowNation/CowSpeak/tree/a44ebfbb8b1a31a5ead65aec6e162512a1b7cf72)

[v7.4 (Major bug fixes, improvements, and changed evaluate method for all types of expessions to DynamicExpresso.Eval April 13, 2020)](https://github.com/CowNation/CowSpeak/tree/71d8f40caf78fdbf9bfe6d41cf28a2781297504a)

[v7.8 (Bug fixes, major optimizations, API changes May 8, 2020)](https://github.com/CowNation/CowSpeak/tree/e5c7222d50f190ef07f464169d6971d17bcc4d10)

Current Version:
[v8 (Added new module system, added byte and ByteArray types, major bug fixes, optimizations, and improvements June 5, 2020)](https://github.com/CowNation/CowSpeak)
