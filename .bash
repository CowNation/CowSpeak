# Build CowSpeak as a library
csc -target:library -r:DynamicExpresso.Core.dll -out:CowSpeak.dll CowSpeak-master/AssemblyInfo.cs CowSpeak-master/Any.cs CowSpeak-master/ByteArray.cs CowSpeak-master/Conditional.cs CowSpeak-master/Conversion.cs CowSpeak-master/CowSpeak.cs CowSpeak-master/Exception.cs CowSpeak-master/Function.cs CowSpeak-master/Functions.cs CowSpeak-master/Lexer.cs CowSpeak-master/Line.cs CowSpeak-master/Scope.cs CowSpeak-master/StaticFunction.cs CowSpeak-master/Syntax.cs CowSpeak-master/Token.cs CowSpeak-master/Type.cs CowSpeak-master/UserFunction.cs CowSpeak-master/Utils.cs CowSpeak-master/Definition.cs CowSpeak-master/Variable.cs CowSpeak-master/csharp-CowConfig.cs CowSpeak-master/Executor.cs CowSpeak-master/FunctionChain.cs CowSpeak-master/Variables.cs # Build as dll

# Run external CowSpeak shell referencing CowSpeak library
mcs -r:CowSpeak.dll -out:Shell.exe main.cs
mono Shell.exe
rm Shell.exe