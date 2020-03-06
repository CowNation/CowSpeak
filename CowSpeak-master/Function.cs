using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;

namespace CowSpeak
{
	internal class Parameter
	{
		public string Name;
		public Type Type;

		public Parameter(Type Type, string Name)
		{
			this.Name = Name;
			this.Type = Type;
		}
	}

	internal abstract class FunctionBase
	{
		public string Name;
		public Type type;
		public string ProperUsage;
		public bool isMethod = false;
		public Parameter[] Parameters; // defined parameters
		public DefinitionType DefinitionType;
		public string Usage
		{
			get
			{
				string def = Name + "(";
				for (int i = 0; i < Parameters.Length; i++)
				{
					def += Parameters[i].Type.Name + " " + Parameters[i].Name;
					if (i < Parameters.Length - 1)
						def += ", ";
				}
				return def + ")";
			}
		}

		public bool isVoid() => type == Type.Void;

		public static Any[] ParseParameters(string s_parameters)
		{
			if (s_parameters == "()")
				return new Any[0]; // no parameters

			List< Any > parameters = new List< Any >();
			s_parameters = s_parameters.Substring(1, s_parameters.Length - 2); // remove parentheses
			
			s_parameters = Utils.ReplaceBetween(Utils.ReplaceBetween(s_parameters, ',', '\"', '\"', (char)0x1a), ',', '(', ')', (char)0x1a).Replace(((char)0x1D).ToString(), " "); // prevent splitting of commas in nested functions & strings

			string[] splitParams = s_parameters.Split(','); // split by each comma (each item is a parameter)

			for (int i = 0; i < splitParams.Length; i++)
			{
				splitParams[i] = splitParams[i].Replace(((char)0x1a).ToString(), ",");

				if (splitParams[i][0] == ',')
					splitParams[i] = splitParams[i].Substring(1, splitParams[i].Length - 1);
			} // splitting has been done so we can revert placeholders back

			foreach (string parameter in splitParams)
			{
				string cleanedUp = "";
				if (parameter != "\"\"" && (parameter[0] == '\"' || parameter[0] == '\'') && (parameter[parameter.Length - 1] == '\"' || parameter[parameter.Length - 1] == '\''))
					cleanedUp = parameter.Substring(1, parameter.Length - 2);
				else
					cleanedUp = parameter;

				cleanedUp = cleanedUp.Replace(((char)0x1f).ToString(), " ").Replace(((char)0x1E).ToString(), ","); // remove quotes/apostrophes & remove string space placeholders
				Token token = null;

				if (parameter.Split('\"').Length - 1 <= 2 && parameter.IndexOf(" ") == -1)
				{
					token = Lexer.ParseToken(parameter, false); // a flaw in the parsing function for strings would take a string chain if it starts and ends with a string as 1 string (this is a janky workaround)
				}

				Type vtype = null;

				if (token == null)
				{
					Line tl = new Line(Lexer.ParseLine(parameter));
					parameters.Add(tl.Exec());
					continue;
				} // unknown identifier, could be an equation waiting to be solved
				else if (token.type == TokenType.VariableIdentifier)
				{
					Variable _var = CowSpeak.GetVariable(token.identifier);
					parameters.Add(new Any(_var.Type, _var.Value));
					continue;
				}
				else if (token.type == TokenType.FunctionCall)
				{
					while ((int)token.identifier[0] < 'A' || (int)token.identifier[0] > 'z')
						token.identifier = token.identifier.Remove(0, 1); // i don't remember why this is here tbh
					FunctionBase func = CowSpeak.GetFunction(token.identifier);
					if (func.type == Type.Void)
						throw new Exception("Cannot pass void function as a parameter");
					parameters.Add(new Any(func.type, func.Execute(token.identifier).Value));
					continue;
				}
				else if (token.type == TokenType.FunctionChain)
				{
					parameters.Add(FunctionChain.Evaluate(token.identifier));
					continue;
				}
				else if (token.type == TokenType.String)
					vtype = Type.String;
				else if (token.type == TokenType.Character)
					vtype = Type.Character;
				else if (token.type == TokenType.Number)
				{
					if (token.identifier.IndexOf(".") != -1)
						vtype = Type.Decimal;
					else
						vtype = Type.Integer;
				}

				if (vtype == null)
					throw new Exception("Unknown type passed as parameter: " + parameter);

				parameters.Add(new Any(vtype, System.Convert.ChangeType(cleanedUp, vtype.rep)));
			}

			return parameters.ToArray();
		}

		public void CheckParameters(List< Any > usedParams)
		{
			if (Parameters.Length != usedParams.Count)
				throw new Exception("Invalid number of parameters passed in FunctionCall: '" + Name + "'");

			for (int i = 0; i < Parameters.Length; i++)
			{
				if (!Conversion.IsCompatible(usedParams[i].Type, Parameters[i].Type))
					throw new Exception("Parameter '" + Parameters[i].Type.Name + " " + Parameters[i].Name + "' is incompatible with '" + usedParams[i].Type.Name + "'");
			}
		}

		// To be overridden
		public abstract Any Execute(string usage);
	}
}