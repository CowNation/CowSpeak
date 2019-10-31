using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;

using System.Reflection;

namespace CowSpeak{
	public class Function {
		public MethodInfo Definition;
		public string Name;
		public VarType type;
		public int requiredParams;
		public string properUsage;
		public bool isMethod;

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

				if (parameter.Split('\"').Length - 1 <= 2 && parameter.IndexOf(" ") == -1){
					token = Lexer.ParseToken(parameter, false); // a flaw in the parsing function for strings would take a string chain if it starts and ends with a string as 1 string (this is a janky workaround)
				}

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

		public Function(string Name, MethodInfo Definition, VarType type, string properUsage, int requiredParams = 0, bool isMethod = false) {
			this.type = type;
			this.Definition = Definition;
   			this.properUsage = properUsage;
			this.Name = Name;
			this.requiredParams = requiredParams;
			this.isMethod = isMethod;
		}

		public Any Execute(string usage) {
			if (usage.IndexOf("(") == -1 || usage.IndexOf(")") == -1)
				CowSpeak.FATAL_ERROR("Invalid usage of function: '" + usage + "'. Proper Usage: " + properUsage);

			string usage_temp = usage;
			usage = usage.Substring(usage.IndexOf("(")); // reduce it to parentheses and params inside of them
			List< Any > parameters = parseParameters(usage).ToList();
			if (isMethod && requiredParams != parameters.Count - 1){
				Variable methodVar = CowSpeak.getVariable(usage_temp.Substring(0, usage_temp.IndexOf(".")));
				parameters.Insert(0, new Any(methodVar.vType, methodVar.Get()));
			}

			if (requiredParams != parameters.Count && (!isMethod || requiredParams != parameters.Count - 1))
				CowSpeak.FATAL_ERROR("Invalid number of parameters passed in FunctionCall: '" + Name + "'. Proper Usage: " + properUsage + " (" + parameters.Count + " given)");

			try{
				return Definition.Invoke(null, new object[]{ parameters.ToArray() }) as Any; // obj is null because the function should be static
			}
			catch (Exception ex) {
				if (ex.GetType().IsAssignableFrom(typeof(InvalidCastException))){
					string givenParams = Name + "(";
					int i = 0;
					foreach (Any _param in parameters){
						if (i == 0 && isMethod){
							i++;
							continue;
						}

						givenParams += _param.vType.Name + "(" + _param.Get().ToString() + ")" + (i == parameters.Count - 1 ? "" : ",");

						i++;
					}
					givenParams += ")";
					CowSpeak.FATAL_ERROR("Invalid parameter types passed in FunctionCall: '" + Name + "'. \nProper Usage: \n" + properUsage + "\nGiven Parameter Types: \n" + givenParams);
				}
				else{
					CowSpeak.FATAL_ERROR("There was an unknown error when executing function: '" + Name + "'. \nProper Usage: \n" + properUsage + "\nError: " + ex.Message);
				}

				return null;
			}
		}
	};

	public class _Function : Attribute {
		public string Name;
		public VarType vType;
		public int requiredParams;
		public string properUsage;
		public bool isMethod;

		public _Function(string Name, string typeName, string properUsage, int requiredParams = 0, bool isMethod = false){
			foreach (VarType type in VarType.GetTypes()){
				if (type.Name == typeName){
					vType = type;
					break;
				}
			}

			this.Name = Name;
			this.properUsage = properUsage;
			this.requiredParams = requiredParams;
			this.isMethod = isMethod;
		}

		public static List< Function > GetFunctions()
        {
            List< Function > functions = new List< Function >();

            MethodInfo[] funcMethods = typeof(Functions).GetMethods(); // Get all methods from the Functions class

            foreach (MethodInfo method in funcMethods)
            {
				_Function functionAttr = (_Function)Attribute.GetCustomAttribute(method, typeof(_Function)); // get attribute for method

                if (functionAttr == null || method == null)
                    continue; // skip method, it does not have this attribute

                functions.Add(new Function(functionAttr.Name, method, functionAttr.vType, functionAttr.properUsage, functionAttr.requiredParams, functionAttr.isMethod));
            }

            return functions;
        } // get a list of all methods from the Functions class that have this attribute
	}
}