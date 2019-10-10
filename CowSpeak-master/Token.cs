using System.Collections.Generic;

namespace CowSpeak{
	public enum TokenType {
		FunctionCall, //0
		Number, //1
		AddOperator, //2
		SubtractOperator, //3
		MultiplyOperator, //4
		DivideOperator, //5
		PowerOperator, //6
		ModOperator, //7
		EqualOperator, //8
		VariableIdentifier, //9
		ParenthesesOperator, //10
		TypeIdentifier,
		String,
		Character,
		IfConditional,
		WhileConditional,
		LoopConditional,
		EndBracket
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

		private bool isStringableVar(Token token){
			if (token.type != TokenType.VariableIdentifier)
				return false;
			Variable _var = CowSpeak.getVariable(token.identifier, false);
			return _var != null && _var.vType == VarType.String;
		}

		private bool isStringableFunc(Token token){
			if (token.type != TokenType.FunctionCall)
				return false;
			Function func = CowSpeak.findFunction(token.identifier, false);
			return func != null && func.type == VarType.String;	
		}

		private bool isCharableVar(Token token){
			if (token.type != TokenType.VariableIdentifier)
				return false;
			Variable _var = CowSpeak.getVariable(token.identifier, false);
			return _var != null && _var.vType == VarType.Character;
		}

		private bool isCharableFunc(Token token){
			if (token.type != TokenType.FunctionCall)
				return false;
			Function func = CowSpeak.findFunction(token.identifier, false);
			return func != null && func.type == VarType.Character;	
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

			string Evaluated = "";
			for (int i = 0; i < toEval.Count; i++){
				string identifier = toEval[i].identifier;

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
				} // don't start chain starting with a char if it has no friends

				if (toEval[i].type == TokenType.String || isStringableVar(toEval[i]) || isStringableFunc(toEval[i]) || toEval[i].type == TokenType.Character || isCharableVar(toEval[i]) || isCharableFunc(toEval[i])){
					// who wants some spaghetti?
					Any result = new Any();
					result.vType = VarType.String;
					List< string > additors = new List< string >();

					if (toEval[i].type == TokenType.String || toEval[i].type == TokenType.Character)
						additors.Add(toEval[i].identifier);
					else if (toEval[i].type == TokenType.VariableIdentifier)
						additors.Add(CowSpeak.getVariable(toEval[i].identifier).Get().ToString()); // stringable variable
					else
						additors.Add(CowSpeak.findFunction(identifier).Execute(identifier).Get().ToString());

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
							additors.Add(CowSpeak.findFunction(toEval[index + 1].identifier).Execute(identifier).Get().ToString()); // stringable func
						else 
							break;
					}

					if (additors.Count == 1)
						result.Set(additors[0]);
					else{
						result.Set(Utils.AddStrings(additors));
					}

					return result;
				}

				if (toEval[i].type == TokenType.VariableIdentifier)
					identifier = CowSpeak.getVariable(identifier).Get().ToString(); // replace variable name with it's value
				if (toEval[i].type == TokenType.WhileConditional || toEval[i].type == TokenType.IfConditional || toEval[i].type == TokenType.EndBracket)
					identifier = "";
				else if (toEval[i].type == TokenType.FunctionCall)
					identifier = CowSpeak.findFunction(identifier).Execute(identifier).Get().ToString(); // replace function call with it's return value
				else if (toEval[i].type == TokenType.Number)
					identifier = identifier.Replace("-", ((char)26).ToString()); // replace negative sign with substitute to work well with Utils.Evaluate

				Evaluated += identifier;
			}

			if (Evaluated == "")
				return new Any(VarType.Integer, 0);

			Any evaluatedValue = new Any();
			try{
				evaluatedValue.vType = VarType.Decimal;
				evaluatedValue.Set(Utils.Evaluate(Evaluated));
				if (((double)evaluatedValue.Get()).ToString().IndexOf(".") == -1)
					evaluatedValue.vType = VarType.Integer; // decimal not found, we can convert to int
			}
			catch{
				CowSpeak.FATAL_ERROR("Could not evaluate expression '" + Evaluated.Replace(((char)26).ToString(), "-") + "'");
			}

			return evaluatedValue;
		}
	};
}