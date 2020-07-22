using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CowSpeak.Exceptions;

namespace CowSpeak
{
	internal class UserFunction : FunctionBase
	{
		List< Line > Definition; // lines contained inside of the function
		int DefinitionOffset; // offset of where the definition is so CowSpeak.CurrentLine is correct
		string DefinedIn; // file function was defined in, may be empty

		public UserFunction(string Name, List< Line > Definition, Parameter[] Parameters, Type type, string ProperUsage, int DefinitionOffset)
		{
			DefinitionType = DefinitionType.User;
			this.ReturnType = type;
			this.Definition = Definition;
			this.Name = Name;
			this.Parameters = Parameters;
			this.DefinitionOffset = DefinitionOffset;
			DefinedIn = Interpreter.CurrentFile;
		}

		private Any ExecuteLines()
		{
			string currentFile = Interpreter.CurrentFile;
			Interpreter.CurrentFile = DefinedIn;

			var returnedValue = Executor.Execute(Definition, DefinitionOffset + 1, true);

			if (ReturnType == Type.Void)
			{
				if (returnedValue != null)
					throw new BaseException("Cannot return a value from a void function");

				return null;
			}

			if (returnedValue == null)
				throw new BaseException("Non-void function '" + Name + "' did not return any valid value");

			if (!Conversion.IsCompatible(returnedValue.Type, ReturnType))
				throw new BaseException("Incompatible return type, ('" + returnedValue.Type.Name + "' is incompatible with '" + ReturnType.Name + "')");

			Interpreter.CurrentFile = currentFile;

			return returnedValue;
		}

		public static Parameter[] ParseDefinitionParams(string s_params)
		{
			if (s_params == "()")
				return null; // no parameters

			List< Parameter > parameters = new List< Parameter >();
			s_params = s_params.Substring(1, s_params.Length - 1); // remove parentheses

			string[] splitParams = Regex.Split(s_params, ","); // split by each comma (each item is a parameter)

			foreach (string parameter in splitParams)
			{
				if (parameter == "")
					continue;

				List< Token > parameterDefinition = Lexer.ParseLine(parameter);
				if (parameterDefinition.Count != 2 || parameterDefinition[0].type != TokenType.TypeIdentifier || parameterDefinition[1].type != TokenType.VariableIdentifier)
					throw new BaseException("Invalid definition of parameter");

				parameters.Add(new Parameter(Type.GetType(parameterDefinition[0].identifier), parameterDefinition[1].identifier));
			}

			return parameters.ToArray();
		}

		public override Any Invoke(string usage)
		{
			if (usage.IndexOf("(") == -1 || usage.IndexOf(")") == -1)
				throw new BaseException("Invalid usage of function: '" + Usage + "'");

			usage = usage.Substring(usage.IndexOf("(")); // reduce it to parentheses and params inside of them

			List< Any > parameters = ParseParameters(usage).ToList();

			CheckParameters(parameters);

			Scope scope = new Scope();

			for (int i = 0; i < Parameters.Length; i++)
			{
				Parameter parameter = Parameters[i];
				Interpreter.Vars.Add(parameter.Name, new Variable(parameter.Type, parameter.Name, parameters[i].Value));
			}

			bool calledRecursively = Interpreter.StackTrace.Count > 0 && Interpreter.StackTrace[Interpreter.StackTrace.Count - 1] == Usage;
			if (calledRecursively)
				throw new BaseException("Recursion is not supported in this version of CowSpeak");

			Interpreter.StackTrace.Add(Usage);
			Any returnedValue = ExecuteLines();
			Interpreter.StackTrace.RemoveAt(Interpreter.StackTrace.Count - 1);

			scope.End();

			return returnedValue;
		}
	}
}