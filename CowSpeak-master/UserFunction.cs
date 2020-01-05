using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace CowSpeak
{
	public class UserFunction : FunctionBase
	{
		List< string > Definition; // lines contained inside of the function
		int DefinitionOffset; // offset of where the definition is so CowSpeak.CurrentLine is correct
		string DefinedIn; // file function was defined in, may be empty

		public UserFunction(string Name, List< string > Definition, Parameter[] Parameters, Type type, string ProperUsage, int DefinitionOffset)
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

		private Any ExecuteLines(List< string > lines)
		{
			string CurrentFile = CowSpeak.CurrentFile;
			CowSpeak.CurrentFile = DefinedIn;

			new Lexer(lines, DefinitionOffset + 1, true);

			string ReturnedLine = Definition[CowSpeak.CurrentLine - DefinitionOffset - 2]; // relative line where Lexer returned

			if (type == Type.Void)
			{
				if (ReturnedLine.IndexOf(Syntax.Statements.Return) == 0 && ReturnedLine != Syntax.Statements.Return)
					throw new Exception("Cannot return a value from a void function");

				return new Any(Type.Integer, 0);
			}

			if (ReturnedLine.IndexOf(Syntax.Statements.Return + " ") != 0)
				throw new Exception("Function is missing a ReturnStatement");

			ReturnedLine = ReturnedLine.Remove(0, Syntax.Statements.Return.Length + 1);

			if (ReturnedLine.Length == 0)
				throw new Exception("ReturnStatement requires a value when the function type is not void");

			Any returnedValue = new Line(Lexer.ParseLine(ReturnedLine)).Exec();

			if (!Conversion.IsCompatible(returnedValue.vType, type))
				throw new Exception("Incompatible return type ('" + returnedValue.vType.Name + "' is incompatible with '" + type.Name + "')");

			CowSpeak.CurrentFile = CurrentFile;

			return returnedValue;
		}

		public static Parameter[] ParseDefinitionParams(string s_params)
		{
			if (s_params == "()")
				return null; // no parameters

			List< Parameter > parameters = new List< Parameter >();
			s_params = s_params.Substring(1, s_params.Length - 1); // remove parentheses

			string[] splitParams = s_params.Split(","); // split by each comma (each item is a parameter)

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

			return new UserFunction(dName, Utils.GetContainedLines(owner.Lines, owner.GetClosingBracket(definitionLine), definitionLine), ParseDefinitionParams(usage.Substring(usage.IndexOf("("))), Utils.GetType(returnType.identifier), returnType.identifier + " " + usage, definitionLine);
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
				CowSpeak.Vars.Insert(0, new Variable(parameter.Type, parameter.Name, parameters[i].Get()));
				//CowSpeak.CreateVariable(new Variable(parameter.Type, parameter.Name, parameters[i].Get()));
			}

			Any returnedValue = ExecuteLines(Definition);

			scope.End();

			return returnedValue;
		}
	}
}