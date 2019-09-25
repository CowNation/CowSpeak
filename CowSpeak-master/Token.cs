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
		ModOperator,
		EqualOperator,
		VariableIdentifier,
		ParenthesesOperator
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
		private Function findFunction(int lineNum, string functionName){
			for (int i = 0; i < CowSpeak.staticFX.Count; i++){
				if (CowSpeak.staticFX[i].funcName == functionName)
					return CowSpeak.staticFX[i];
			}

			Utils.FATAL_ERROR(lineNum + 1, "Function " + functionName + " not found");
			Functions.VOID_exit();
			return null;
		} // find fuction with matching name
		public List< Token > tokens;

		public TokenLine(List< Token > tt){
			tokens = tt;
		}

		public float Exec(int lineNum, List< Variable > Vars){
			List< Token > toEval = tokens;

			for (int i = 0; i < tokens.Count; i++){
				if (tokens[i].type == TokenType.EqualOperator){
					toEval = tokens.GetRange(i + 1, (tokens.Count) - (i + 1));
					break;
				}
			} // remove the equal sign and everything to the left

			string Evaluated = "";
			for (int i = 0; i < toEval.Count; i++){
				string identifier = toEval[i].identifier;

				if (toEval[i].type == TokenType.VariableIdentifier){
					identifier = Vars[Utils.getVariable(lineNum, Vars, identifier)].Value.ToString();
				} // replace variable name with it's value
				else if (toEval[i].type == TokenType.FunctionCall){
					identifier = findFunction(lineNum, identifier).FuncDef().ToString();
				} // replace function call with it's return value

				Evaluated += identifier;
			}

			float evaluatedValue = 0;
			try{
				evaluatedValue = (float)Utils.Evaluate(Evaluated);
			}
			catch{
				Utils.FATAL_ERROR(lineNum + 1, "Could not evaluate expression '" + Evaluated + "'");
			}

			return evaluatedValue;
		}
	};
}