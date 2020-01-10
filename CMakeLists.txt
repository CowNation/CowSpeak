CMAKE_MINIMUM_REQUIRED(VERSION 3.10)

PROJECT(CowSpeak LANGUAGES CSharp)

SET(CMAKE_CSharp_FLAGS "/langversion:6")

SET(CMAKE_SUPPRESS_REGENERATION true)

ADD_EXECUTABLE(CowSpeak
	CowSpeak-master/Any.cs
	CowSpeak-master/ByteArray.cs
	CowSpeak-master/Conditional.cs
	CowSpeak-master/Conversion.cs
	CowSpeak-master/CowSpeak.cs
	CowSpeak-master/csharp-CowConfig.cs
	CowSpeak-master/Definition.cs
	CowSpeak-master/Error.cs
	CowSpeak-master/Evaluate.cs
	CowSpeak-master/Executor.cs
	CowSpeak-master/Function.cs
	CowSpeak-master/Functions.cs
	CowSpeak-master/Lexer.cs
	CowSpeak-master/Line.cs
	CowSpeak-master/Scope.cs
	CowSpeak-master/StaticFunction.cs
	CowSpeak-master/Syntax.cs
	CowSpeak-master/Token.cs
	CowSpeak-master/Type.cs
	CowSpeak-master/UserFunction.cs
	CowSpeak-master/Utils.cs
	CowSpeak-master/Variable.cs
	main.cs)

SET_PROPERTY(TARGET CowSpeak PROPERTY VS_DOTNET_REFERENCES
    "Microsoft.CSharp"
    "System"
    "System.Core"
    "System.Data")