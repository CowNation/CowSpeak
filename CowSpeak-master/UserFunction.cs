using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace CowSpeak{
	public class UserFunction : FunctionBase {
		List< string > Definition; // lines contained inside of the function
		int definitionOffset; // offset of where the definition is so CowSpeak.currentLine is correct

		public UserFunction(string Name, List< string > Definition, Parameter[] Parameters, Type type, string properUsage, int definitionOffset) {
			this.definitionType = DefinitionType.User;
			this.type = type;
			this.Definition = Definition;
   			this.properUsage = properUsage;
			this.Name = Name;
			this.Parameters = Parameters;
			this.definitionOffset = definitionOffset;
		}

		private Any ExecuteLines(List< string > lines){
			new Lexer(lines, CowSpeak.shouldDebug, definitionOffset + 1, true);
			string ReturnedLine = Definition[CowSpeak.currentLine - definitionOffset - 2]; // relative line where Lexer returned

			if (ReturnedLine.IndexOf(Syntax.Return + " ") != 0) // missing ReturnStatement as 1st token
				CowSpeak.FatalError("Function is missing a ReturnStatement");

			ReturnedLine = ReturnedLine.Remove(0, Syntax.Return.Length + 1);

			return new TokenLine(Lexer.ParseLine(ReturnedLine)).Exec();
		}

		public static Parameter[] ParseDefinitionParams(string s_params){
			if (s_params == "()")
				return null; // no parameters

			List< Parameter > parameters = new List< Parameter >();
			s_params = s_params.Substring(1, s_params.Length - 1); // remove parentheses

			string[] splitParams = s_params.Split(","); // split by each comma (each item is a parameter)

			foreach (string parameter in splitParams){
				if (parameter == "")
					continue;

				List< Token > parameterDefinition = Lexer.ParseLine(parameter);
				if (parameterDefinition.Count != 2 || parameterDefinition[0].type != TokenType.TypeIdentifier || parameterDefinition[1].type != TokenType.VariableIdentifier)
					CowSpeak.FatalError("Invalid definition of parameter");

				parameters.Add(new Parameter(Utils.GetType(parameterDefinition[0].identifier), parameterDefinition[1].identifier));
			}

			return parameters.ToArray();
		}

		public static UserFunction ParseDefinition(Lexer owner, int definitionLine, Token returnType, string usage){
			usage = usage.Substring(0, usage.Length - 2); // remove StartBracket

			string dName = usage.Substring(0, usage.IndexOf("(")); // text before first '('

			return new UserFunction(dName, Utils.GetContainedLines(owner.Lines, owner.GetClosingBracket(definitionLine), definitionLine), ParseDefinitionParams(usage.Substring(usage.IndexOf("("))), Utils.GetType(returnType.identifier), returnType.identifier + " " + usage, definitionLine);
		}

		public override Any Execute(string usage) {
			if (usage.IndexOf("(") == -1 || usage.IndexOf(")") == -1)
				CowSpeak.FatalError("Invalid usage of function: '" + usage + "'\nProper Usage: " + properUsage);

			usage = usage.Substring(usage.IndexOf("(")); // reduce it to parentheses and params inside of them
			List< Any > parameters = ParseParameters(usage).ToList();

			CheckParameters(parameters);

			try{
				RestrictedScope scope = new RestrictedScope();
				for (int i = 0; i < Parameters.Length; i++){
					Parameter parameter = Parameters[i];
					CowSpeak.CreateVariable(new Variable(parameter.Type, parameter.Name, parameters[i].Get()));
				}
				Any returnedValue = ExecuteLines(Definition);
				scope.End();
				return returnedValue;
			}
			catch (Exception ex) {
				if (ex.GetType().IsAssignableFrom(typeof(InvalidCastException))){
					string givenParams = Name + "(";
					int i = 0;
					foreach (Any _param in parameters){
						givenParams += _param.vType.Name + "(" + _param.Get().ToString() + ")" + (i == parameters.Count - 1 ? "" : ","); // it just works

						i++;
					}
					givenParams += ")";
					CowSpeak.FatalError("Invalid parameter types passed in FunctionCall: '" + Name + "'. \nProper Usage: \n" + properUsage + "\nGiven Parameter Types: \n" + givenParams);
				}
				else{
					CowSpeak.FatalError("There was an unknown error when executing function: '" + Name + "'. \nProper Usage: \n" + properUsage + "\nError: " + ex.Message);
				}

				return null;
			}
		}
	};
}