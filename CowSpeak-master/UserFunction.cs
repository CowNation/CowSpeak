using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace CowSpeak
{
	internal class UserFunction : FunctionBase
	{
		List< Line > Definition; // lines contained inside of the function
		int DefinitionOffset; // offset of where the definition is so CowSpeak.CurrentLine is correct
		string DefinedIn; // file function was defined in, may be empty

		public UserFunction(string Name, List< Line > Definition, Parameter[] Parameters, Type type, string ProperUsage, int DefinitionOffset)
		{
			this.DefinitionType = DefinitionType.User;
			this.type = type;
			this.Definition = Definition;
   			this.ProperUsage = ProperUsage;
			this.Name = Name;
			this.Parameters = Parameters;
			this.DefinitionOffset = DefinitionOffset;
			this.DefinedIn = CowSpeak.CurrentFile;
		}

		private Any ExecuteLines()
		{
			string CurrentFile = CowSpeak.CurrentFile;
			CowSpeak.CurrentFile = DefinedIn;

			var ReturnedValue = Executor.Execute(Definition, DefinitionOffset + 1, true);

			if (type == Type.Void)
			{
				if (ReturnedValue != null)
					throw new Exception("Cannot return a value from a void function");

				return null; // TODO: Return null
			}

			if (ReturnedValue == null)
				throw new Exception("Non-void function '" + Name + "' did not return any valid value");

			if (!Conversion.IsCompatible(ReturnedValue.Type, type))
				throw new Exception("Incompatible return type, ('" + ReturnedValue.Type.Name + "' is incompatible with '" + type.Name + "')");

			CowSpeak.CurrentFile = CurrentFile;

			return ReturnedValue;
		}

		public static Parameter[] ParseDefinitionParams(string s_params)
		{
			if (s_params == "()")
				return null; // no parameters

			List< Parameter > parameters = new List< Parameter >();
			s_params = s_params.Substring(1, s_params.Length - 1); // remove parentheses

			string[] splitParams = s_params.Split(','); // split by each comma (each item is a parameter)

			foreach (string parameter in splitParams)
			{
				if (parameter == "")
					continue;

				List< Token > parameterDefinition = Lexer.ParseLine(parameter);
				if (parameterDefinition.Count != 2 || parameterDefinition[0].type != TokenType.TypeIdentifier || parameterDefinition[1].type != TokenType.VariableIdentifier)
					throw new Exception("Invalid definition of parameter");

				parameters.Add(new Parameter(Utils.GetType(parameterDefinition[0].identifier), parameterDefinition[1].identifier));
			}

			return parameters.ToArray();
		}

		public static UserFunction ParseDefinition(Lexer owner, int definitionLine, Token returnType, string usage)
		{
			usage = usage.Substring(0, usage.Length - 2); // remove StartBracket

			string dName = usage.Substring(0, usage.IndexOf("(")); // text before first '('

			return new UserFunction(dName, Utils.pGetContainedLines(owner.Lines, Executor.GetClosingBracket(owner.Lines, definitionLine), definitionLine), ParseDefinitionParams(usage.Substring(usage.IndexOf("("))), Utils.GetType(returnType.identifier), returnType.identifier + " " + usage, definitionLine);
		}

		public override Any Execute(string usage)
		{
			if (usage.IndexOf("(") == -1 || usage.IndexOf(")") == -1)
				throw new Exception("Invalid usage of function: '" + Usage + "'");

			usage = usage.Substring(usage.IndexOf("(")); // reduce it to parentheses and params inside of them

			List< Any > parameters = ParseParameters(usage).ToList();

			CheckParameters(parameters);

			Scope scope = new Scope();

			for (int i = 0; i < Parameters.Length; i++)
			{
				Parameter parameter = Parameters[i];
				CowSpeak.Vars.Insert(0, new Variable(parameter.Type, parameter.Name, parameters[i].Value));
			}

			bool isBeingCalledRecursively = CowSpeak.StackTrace.Count > 0 && CowSpeak.StackTrace[CowSpeak.StackTrace.Count - 1] == Usage;
			if (isBeingCalledRecursively)
				throw new Exception("Recursion is not supported in this version of CowSpeak");

			CowSpeak.StackTrace.Add(Usage);
			Any returnedValue = ExecuteLines();
			CowSpeak.StackTrace.RemoveAt(CowSpeak.StackTrace.Count - 1);

			scope.End();

			return returnedValue;
		}
	}
}