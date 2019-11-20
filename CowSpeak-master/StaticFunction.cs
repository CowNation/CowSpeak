using System;
using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CowSpeak {
	public class StaticFunction : FunctionBase {
		public MethodInfo Definition;

		public StaticFunction(string Name, MethodInfo Definition, Type type, Parameter[] Parameters, bool isMethod = false) {
			this.definitionType = DefinitionType.Language;
			this.type = type;
			this.Definition = Definition;

			string Params = "";
			for (int i = 0; i < Parameters.Length; i++){
				Params += Parameters[i].Type + " " + Parameters[i].Name;
				if (i != Parameters.Length - 1)
					Params += ", ";
			}

   			this.properUsage = type.Name + " " + Name + "(" + Params +  ")";
			this.Name = Name;
			this.Parameters = Parameters;
			this.isMethod = isMethod;
		}

		public override Any Execute(string usage) {
			if (usage.IndexOf("(") == -1 || usage.IndexOf(")") == -1)
				CowSpeak.FatalError("Invalid usage of function: '" + usage + "'\nProper Usage: " + properUsage);

			string usage_temp = usage;
			usage = usage.Substring(usage.IndexOf("(")); // reduce it to parentheses and params inside of them
			List< Any > parameters = ParseParameters(usage).ToList();
			if (isMethod && Parameters.Length != parameters.Count - 1){
				Variable methodVar = CowSpeak.GetVariable(usage_temp.Substring(0, usage_temp.IndexOf(".")));
				parameters.Insert(0, new Any(methodVar.vType, methodVar.Get()));
			}

			CheckParameters(parameters);

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
					CowSpeak.FatalError("Invalid parameter types passed in FunctionCall: '" + Name + "'. \nProper Usage: \n" + properUsage + "\nGiven Parameter Types: \n" + givenParams);
				}
				else{
					CowSpeak.FatalError("There was an unknown error when executing function: '" + Name + "'. \nProper Usage: \n" + properUsage + "\nError: " + ex.Message);
				}

				return null;
			}
		}
	};

	public class FunctionAttr : Attribute {
		public string Name;
		public Type vType = null;
		public bool isMethod;
		public List< Parameter > Parameters = new List< Parameter >();

		public FunctionAttr(string Name, string typeName, string _Parameters, bool isMethod = false){
			foreach (Type type in Type.GetTypes()){
				if (type.Name == typeName){
					vType = type;
					break;
				}
			}

			this.Name = Name;
			this.isMethod = isMethod;

			if (_Parameters != ""){
				string[] SplitParameters = _Parameters.Split(",");
				foreach (string Parameter in SplitParameters){
					List< Token > Tokens = Lexer.ParseLine(Parameter);
					Parameters.Add(new Parameter(Utils.GetType(Tokens[0].identifier), Tokens[1].identifier));
				}
			}
		}

		public static List< FunctionBase > GetStaticFunctions() {
            List< FunctionBase > functions = new List< FunctionBase >();

            MethodInfo[] funcMethods = typeof(Functions).GetMethods(); // Get all methods from the Functions class

            foreach (MethodInfo method in funcMethods)
            {
				FunctionAttr functionAttr = (FunctionAttr)Attribute.GetCustomAttribute(method, typeof(FunctionAttr)); // get attribute for method

                if (functionAttr == null || method == null)
                    continue; // skip method, it does not have this attribute

                functions.Add(new StaticFunction(functionAttr.Name, method, functionAttr.vType, functionAttr.Parameters.ToArray(), functionAttr.isMethod));
            }

            return functions;
        } // get a list of all methods from the Functions class that have this attribute
	}
}