using System.Threading;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Globalization;
using CowSpeak.Exceptions;
using System;

namespace CowSpeak
{
	public class Parameter
	{
		public string Name;
		public Type Type;

		public Parameter(Type type, string name)
		{
			this.Name = name;
			this.Type = type;
		}
	}

	public abstract class BaseFunction
	{
		public string Name;
		public Type ReturnType;
		public bool IsMethod = false;
		public Parameter[] Parameters;
		internal DefinitionType DefinitionType;

		public string Usage
		{
			get
			{
				string def = ReturnType.Name + " " + Name + "(";
				for (int i = 0; i < Parameters.Length; i++)
				{
					def += Parameters[i].Type.Name + " " + Parameters[i].Name;
					if (i < Parameters.Length - 1)
						def += ", ";
				}
				return def + ")";
			}
		}

		public static bool IsFunctionCall(string token)
		{
			if (token.IndexOf("(") <= 0 || Utils.GetClosingParenthesis(token) != token.Length - 1)
				return false;

			string functionName = token.Substring(0, token.IndexOf("("));

			if (functionName.OccurrencesOf(".") > 1) // it's not a FunctionCall, it's probably a FunctionChain
				return false;

			return Utils.IsValidFunctionName(functionName.Replace(".", "_"));
		}

		public static Any[] ParseParameters(string s_parameters)
		{
			if (s_parameters == "()")
				return new Any[0]; // no parameters

			List< Any > parameters = new List< Any >();
			s_parameters = s_parameters.Substring(1, s_parameters.Length - 2); // remove parentheses
			
			s_parameters = Utils.ReplaceBetween(s_parameters, ',', '(', ')', (char)0x1a); // prevent splitting of commas in nested parentheses

			string[] splitParams = Regex.Split(s_parameters, ","); // split by each comma/space (each item is a parameter)

			for (int i = 0; i < splitParams.Length; i++)
			{
				splitParams[i] = splitParams[i].Replace(((char)0x1a).ToString(), ",");

				if (splitParams[i][0] == ',')
					splitParams[i] = splitParams[i].Substring(1, splitParams[i].Length - 1);
			} // splitting has been done so we can revert placeholders back

			for (int i = 0; i < splitParams.Length; i++)
			{
				string parameter = splitParams[i];

				while (parameter.Length > 0 && parameter[0] == ' ')
					parameter = parameter.Remove(0, 1); // remove beginning spaces

				while (parameter.Length > 0 && parameter[parameter.Length - 1] == ' ')
					parameter = parameter.Remove(parameter.Length - 1, 1); // remove trailing spaces

                string cleanedUp;
                if ((parameter[0] == '\"' || parameter[0] == '\'') && (parameter[parameter.Length - 1] == '\"' || parameter[parameter.Length - 1] == '\''))
					cleanedUp = parameter.Substring(1, parameter.Length - 2); // remove quotes/apostrophes
				else
					cleanedUp = parameter;

				Token token = null;

				if (parameter.OccurrencesOf("\"") <= 2)
					token = Lexer.ParseToken(parameter, false); // a flaw in the parsing function for strings would take a string chain if it starts and ends with a string as 1 string (this is a janky workaround)

				Type vtype = null;

				if (token == null)
				{
					// unknown identifier, could be an equation waiting to be solved
					Line tl = new Line(Lexer.ParseLine(parameter));
					parameters.Add(tl.Execute());
					continue;
				}
				else if (token.type == TokenType.VariableIdentifier)
				{
					Variable _var = Interpreter.Vars[token.identifier];
					parameters.Add(new Any(_var.Type, _var.Value));
					continue;
				}
				else if (token.type == TokenType.FunctionCall)
				{
					while (token.identifier[0] < 'A' || token.identifier[0] > 'z')
						token.identifier = token.identifier.Remove(0, 1); // i don't remember why this is here tbh
					BaseFunction func = Interpreter.Functions[token.identifier];
					if (func.ReturnType == Types.Void)
						throw new BaseException("Cannot pass void function as a parameter");
					parameters.Add(new Any(func.ReturnType, func.Invoke(token.identifier).Value));
					continue;
				}
				else if (token.type == TokenType.FunctionChain)
				{
					parameters.Add(FunctionChain.Evaluate(token.identifier));
					continue;
				}
				else if (token.type == TokenType.StringLiteral || token.type == TokenType.CharacterLiteral)
				{
					switch (token.type)
					{
						case TokenType.StringLiteral:
							vtype = Types.String;
							break;
						case TokenType.CharacterLiteral:
							vtype = Types.Character;
							break;
					}

					if (cleanedUp.Length > 2)
						cleanedUp = cleanedUp.FromBase64().Replace("\\\"", "\"").Replace("\\'", "\'"); // convert encoded base64 text given it's not an empty literal & do literal conversions (\" -> ") and (\' -> ')
				}
				else if (token.type == TokenType.Integer64Literal || token.type == TokenType.IntegerLiteral || token.type == TokenType.DecimalLiteral)
				{
					if (Interpreter.Definitions.ContainsKey(cleanedUp))
						cleanedUp = Interpreter.Definitions[cleanedUp].To;

					// get the appropriate type for the literal
					switch (token.type)
					{
						case TokenType.DecimalLiteral:
							vtype = Types.Decimal;
							break;
						case TokenType.IntegerLiteral:
							vtype = Types.Integer;
							if (Utils.IsHexadecimal(cleanedUp))
								cleanedUp = int.Parse(cleanedUp.Substring(2), NumberStyles.HexNumber).ToString();
							break;
						case TokenType.Integer64Literal:
							vtype = Types.Integer64;
							if (Utils.IsHexadecimal(cleanedUp))
								cleanedUp = long.Parse(cleanedUp.Substring(2), NumberStyles.HexNumber).ToString();
							break;
					}
				}
				else if (token.type == TokenType.BooleanLiteral)
					vtype = Types.Boolean;

				if (vtype == null)
					throw new BaseException("Unknown type passed as parameter: " + token.type);

				parameters.Add(new Any(vtype, Convert.ChangeType(cleanedUp, vtype.CSharpType)));
			}

			return parameters.ToArray();
		}

		public static void CheckParameters(string name, Parameter[] parameters, List<Any> usedParams)
		{
			bool validUsage = parameters.Length == usedParams.Count;

			if (validUsage)
			{
				for (int i = 0; i < parameters.Length; i++)
				{
					if (!Conversion.IsCompatible(usedParams[i].Type, parameters[i].Type))
					{
						validUsage = false;
						break;
					}
				}
			}
			else
			{
				var usage = name + "(";
				for (int i = 0; i < parameters.Length; i++)
				{
					usage += parameters[i].Type.Name + " " + parameters[i].Name;
					if (i < parameters.Length - 1)
						usage += ", ";
				}
				usage = usage + ")";

				var givenUsage = name + "(";
				for (int i = 0; i < usedParams.Count; i++)
				{
					givenUsage += usedParams[i].Type.Name;
					if (i < usedParams.Count - 1)
						givenUsage += ", ";
				}
				givenUsage = givenUsage + ")";

				throw new BaseException("Invalid usage of function: " + givenUsage + "; Correct usage: " + usage);
			}
		}


		public void CheckParameters(List< Any > usedParams)
		{
			bool validUsage = Parameters.Length == usedParams.Count;

			if (validUsage)
			{
				for (int i = 0; i < Parameters.Length; i++)
				{
					if (!Conversion.IsCompatible(usedParams[i].Type, Parameters[i].Type))
					{
						validUsage = false;
						break;
					}
				}
			}
			else
			{
				var usage = Name + "(";
				for (int i = 0; i < Parameters.Length; i++)
				{
					usage += Parameters[i].Type.Name + " " + Parameters[i].Name;
					if (i < Parameters.Length - 1)
						usage += ", ";
				}
				usage = usage + ")";

				var givenUsage = Name + "(";
				for (int i = 0; i < usedParams.Count; i++)
				{
					givenUsage += usedParams[i].Type.Name;
					if (i < usedParams.Count - 1)
						givenUsage += ", ";
				}
				givenUsage = givenUsage + ")";

				throw new BaseException("Invalid usage of function: " + givenUsage + "; Correct usage: " + usage);
			}
		}

		// To be overridden
		public abstract Any Invoke(string usage);
	}
}