using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

namespace CowSpeak{
	public class Function {
		public Func<Any[], Any> FuncDef;
		public string funcName;
		public VarType type;
		public int requiredParams;
		public string properUsage = "";
		public bool isMethod = false;

		public bool isVoid(){
			return type == VarType.Void;
		}

		public static Any[] parseParameters(string s_parameters){
			if (s_parameters == "()")
				return new Any[0]; // no parameters

			List< Any > parameters = new List< Any >();
			s_parameters = s_parameters.Substring(1, s_parameters.Length - 2); // remove parentheses
			
			s_parameters = Utils.substituteBetween(s_parameters, ',', '(', ')', (char)0x1a).Replace(((char)0x1D).ToString(), " "); // prevent splitting of commas in nested functions & strings

			string[] splitParams = s_parameters.Split(","); // split by each comma (each item is a parameter)

			for (int i = 0; i < splitParams.Length; i++){
				splitParams[i] = splitParams[i].Replace(((char)0x1a).ToString(), ",");

				if (splitParams[i][0] == ',')
					splitParams[i] = splitParams[i].Substring(1, splitParams[i].Length - 1);
			} // splitting has been done so we can revert placeholders back

			foreach (string parameter in splitParams){
				string cleanedUp = "";
				if (parameter != "\"\"" && (parameter[0] == '\"' || parameter[0] == '\'') && (parameter[parameter.Length - 1] == '\"' || parameter[parameter.Length - 1] == '\''))
					cleanedUp = parameter.Substring(1, parameter.Length - 2);
				else
					cleanedUp = parameter;

				cleanedUp = cleanedUp.Replace(((char)0x1f).ToString(), " ").Replace(((char)0x1E).ToString(), ","); // remove quotes/apostrophes & remove string space placeholders
				Token token = null;

				if (parameter.Split('\"').Length - 1 <= 2)
					token = Lexer.ParseToken(parameter, false); // a flaw in the parsing function for strings would take a string chain if it starts and ends with a string as 1 string (this is a janky workaround)

				VarType vtype = null;

				if (token == null){
					TokenLine tl = new TokenLine(Lexer.ParseLine(parameter));
					parameters.Add(tl.Exec());
					continue;
				} // unknown identifier, could be an equation waiting to be solved
				else if (token.type == TokenType.VariableIdentifier){
					Variable _var = CowSpeak.getVariable(token.identifier);
					parameters.Add(new Any(_var.vType, _var.Get()));
					continue;
				}
				else if (token.type == TokenType.FunctionCall){
					while ((int)token.identifier[0] < 'A' || (int)token.identifier[0] > 'z'){
						token.identifier = token.identifier.Remove(0, 1);
					}
					Function func = CowSpeak.findFunction(token.identifier);
					parameters.Add(new Any(func.type, func.Execute(token.identifier).Get()));
					continue;
				}
				else if (token.type == TokenType.String)
					vtype = VarType.String;
				else if (token.type == TokenType.Character)
					vtype = VarType.Character;
				else if (token.type == TokenType.Number){
					if (token.identifier.IndexOf(".") != -1)
						vtype = VarType.Decimal;
					else
						vtype = VarType.Integer;
				}

				if (vtype == null)
					CowSpeak.FATAL_ERROR("Unknown type passed as parameter: " + parameter);


				parameters.Add(new Any(vtype, Convert.ChangeType(cleanedUp, vtype.rep)));
			}

			return parameters.ToArray();
		}

		public Function(string FunctionName, Func<Any[], Any> FunctionDefinition, VarType type, string properUsage, int requiredParams = 0, bool isMethod = false) {
			this.type = type;
			FuncDef = FunctionDefinition;
			this.properUsage = properUsage;
			funcName = FunctionName;
			this.requiredParams = requiredParams;
			this.isMethod = isMethod;
		}

		public Any Execute(string usage) {
			Variable methodVar = null;
			if (isMethod)
				methodVar = CowSpeak.getVariable(usage.Substring(0, usage.IndexOf(".")));

			if (usage.IndexOf("(") == -1 || usage.IndexOf(")") == -1)
				CowSpeak.FATAL_ERROR("Invalid usage of function: '" + usage + "'. Proper Usage: " + properUsage);

			usage = usage.Substring(usage.IndexOf("(")); // reduce it to parentheses and params inside of them
			List< Any > parameters = parseParameters(usage).ToList();
			if (isMethod)
				parameters.Insert(0, new Any(VarType.String, methodVar.Get()));

			if (requiredParams != parameters.Count && (!isMethod || requiredParams != parameters.Count - 1))
				CowSpeak.FATAL_ERROR("Invalid number of parameters passed in FunctionCall: '" + funcName + "'. Proper Usage: " + properUsage + " (" + parameters.Count + " given)");

			try{
				return FuncDef(parameters.ToArray());
			}
			catch (Exception ex) {
				if (ex.GetType().IsAssignableFrom(typeof(InvalidCastException)))
					CowSpeak.FATAL_ERROR("Invalid parameter types passed in FunctionCall: '" + funcName + "'. Proper Usage: " + properUsage);
				else{
					CowSpeak.FATAL_ERROR("There was an unknown error when executing function: '" + funcName + "'. Proper Usage: " + properUsage + ex.Message);
				}

				return null;
			}
		}
	};
}