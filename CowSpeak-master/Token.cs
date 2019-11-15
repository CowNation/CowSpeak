using System.Collections.Generic;

namespace CowSpeak{
	public enum TokenType {
		FunctionCall,
		Number,
		AddOperator,
		SubtractOperator,
		MultiplyOperator,
		DivideOperator,
		PowerOperator,
		ModuloOperator,
		EqualOperator,
		VariableIdentifier,
		TypeIdentifier,
		String,
		Character,
		IfConditional,
		ElseConditional,
		WhileConditional,
		LoopConditional,
		EndBracket,
		IsEqualOperator,
		IsNotEqualOperator,
		IsGreaterThanOperator,
		IsLessThanOperator,
		DeleteIdentifier
	};

	public class Token {
		public TokenType type;
		public string identifier;
		public Token(TokenType tt, string id){
			type = tt;
			identifier = id;
		}
	};

	public class TokenLine {
		public List< Token > tokens;

		public TokenLine(List< Token > tt){
			tokens = tt;
		}

		private static bool isStringableVar(Token token){
			if (token.type != TokenType.VariableIdentifier)
				return false;
			Variable _var = CowSpeak.getVariable(token.identifier, false);
			return _var != null && _var.vType == VarType.String;
		}

		private static bool isStringableFunc(Token token){
			if (token.type != TokenType.FunctionCall)
				return false;
			Function func = CowSpeak.findFunction(token.identifier, false);
			return func != null && func.type == VarType.String;	
		}

		private static bool isCharableVar(Token token){
			if (token.type != TokenType.VariableIdentifier)
				return false;
			Variable _var = CowSpeak.getVariable(token.identifier, false);
			return _var != null && _var.vType == VarType.Character;
		}

		private static bool isCharableFunc(Token token){
			if (token.type != TokenType.FunctionCall)
				return false;
			Function func = CowSpeak.findFunction(token.identifier, false);
			return func != null && func.type == VarType.Character;	
		}

		private static Any FixCharChain(List< Token > toEval, int i, string identifier){
			if ((toEval[i].type == TokenType.Character || isCharableFunc(toEval[i]) || isCharableVar(toEval[i])) && i == toEval.Count - 1){
				Any result = new Any();
				result.vType = VarType.Character;

				if (toEval[i].type == TokenType.Character)
					result.Set(toEval[i].identifier[0]);
				else if (toEval[i].type == TokenType.FunctionCall)
					result.Set((char)CowSpeak.findFunction(identifier).Execute(identifier).Get());
				else
					result.Set((char)CowSpeak.getVariable(toEval[i].identifier).Get()); // stringable variable

				return result;
			}

			return null;
		} // don't start chain starting with a char if it has no friends

		private static Any TryStrChain(List< Token > toEval, int i, string identifier){
			if (toEval[i].type == TokenType.String || isStringableVar(toEval[i]) || isStringableFunc(toEval[i]) || toEval[i].type == TokenType.Character || isCharableVar(toEval[i]) || isCharableFunc(toEval[i])){
				// who wants some spaghetti?
				Any result = new Any();
				result.vType = VarType.String;
				List< string > additors = new List< string >();

				if (toEval[i].type == TokenType.String || toEval[i].type == TokenType.Character)
					additors.Add(toEval[i].identifier);
				else if (toEval[i].type == TokenType.VariableIdentifier)
					additors.Add(CowSpeak.getVariable(toEval[i].identifier).Get().ToString()); // stringable variable
				else if (toEval[i].type == TokenType.FunctionCall)
					additors.Add(CowSpeak.findFunction(identifier).Execute(identifier).Get().ToString());
				else
					CowSpeak.FATAL_ERROR("An unknown error has occured in 'TryStrChain'");

				int index = i;
				while (true){
					index++;
					if (index != i + 1)
						index++;
					
					if (!Utils.isIndexValid(index + 1, toEval) || !Utils.isIndexValid(index, toEval) || toEval[index].type != TokenType.AddOperator)
						break;
					
					if (toEval[index + 1].type == TokenType.VariableIdentifier)
						additors.Add(CowSpeak.getVariable(toEval[index + 1].identifier).Get().ToString());
					else if (toEval[index + 1].type == TokenType.String || toEval[index + 1].type == TokenType.Number || toEval[index + 1].type == TokenType.Character)
						additors.Add(toEval[index + 1].identifier);
					else if (toEval[index + 1].type == TokenType.FunctionCall)
						additors.Add(CowSpeak.findFunction(toEval[index + 1].identifier).Execute(toEval[index + 1].identifier).Get().ToString()); // stringable func
					else 
						break;
				}

				if (additors.Count == 1)
					result.Set(additors[0]);
				else
					result.Set(Utils.AddStrings(additors));

				return result;
			}

			return null;
		}

		public Any Exec(){
			List< Token > toEval = tokens;

			for (int i = 0; i < tokens.Count; i++){
				if (tokens[i].type == TokenType.FunctionCall){
					if (CowSpeak.findFunction(tokens[i].identifier).isVoid()){
						if (Utils.isIndexValid(i - 1, tokens) && Utils.isOperator(tokens[i-1].type)){
							CowSpeak.FATAL_ERROR("Cannot perform operation: '" + tokens[i-1].identifier + "' on void function");
						}
						if (Utils.isIndexValid(i + 1, tokens) && Utils.isOperator(tokens[i+1].type)){
							CowSpeak.FATAL_ERROR("Cannot perform operation: '" + tokens[i+1].identifier + "' on void function");
						}
					}
				
				}
			}

			for (int i = 0; i < tokens.Count; i++){
				if (tokens[i].type == TokenType.EqualOperator){
					toEval = tokens.GetRange(i + 1, (tokens.Count) - (i + 1));
					break;
				}
			} // remove the equal sign and everything to the left

			List< Token > Evaluated = new List< Token >();
			for (int i = 0; i < toEval.Count; i++){
				string identifier = toEval[i].identifier;
				TokenType type = toEval[i].type;

				Any fixCharChain = FixCharChain(toEval, i, identifier);
				if (fixCharChain != null)
					return fixCharChain;

				Any strChain = TryStrChain(toEval, i, identifier);
				if (strChain != null)
					return strChain;

				if (toEval[i].type == TokenType.VariableIdentifier){
					type = TokenType.Number;
					identifier = CowSpeak.getVariable(identifier).Get().ToString(); // replace variable name with it's value
				} // TOFIX
				if (toEval[i].type == TokenType.WhileConditional || toEval[i].type == TokenType.IfConditional || toEval[i].type == TokenType.EndBracket)
					continue;
				else if (toEval[i].type == TokenType.FunctionCall){
					type = TokenType.Number;
					Function func = CowSpeak.findFunction(identifier);
					if (toEval.Count == 1)
						return new Any(func.type, func.Execute(identifier).Get());
					identifier = func.Execute(identifier).Get().ToString(); // replace function call with it's return value
				}
				
				Evaluated.Add(new Token(type, Utils.FixBoolean(identifier)));
			}

			if (Evaluated.Count == 0)
				return new Any(VarType.Integer, 0);

			Any evaluatedValue = new Any();
			evaluatedValue.vType = VarType.Decimal;
			evaluatedValue.Set(Evaluate.EvaluateTokens(Evaluated));
			if (((double)evaluatedValue.Get()).ToString().IndexOf(".") == -1)
				evaluatedValue.vType = VarType.Integer; // decimal not found, we can convert to int

			return evaluatedValue;
		}
	};
}