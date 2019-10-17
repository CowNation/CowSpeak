using System.Collections.Generic;
using System;

namespace CowSpeak{
	public static class Evaluate{
		public static double EvaluateTokens(List< Token > Tokens){
			int index = 0;

			// Order of operations: solve exponents first
			while (true){
				Token token = Tokens[index];

				if (token.type == TokenType.PowerOperator && Utils.isIndexValid(index - 1, Tokens) && Utils.isIndexValid(index + 1, Tokens)){
					Token _operator = Tokens[index];
					double _left = Convert.ToDouble(Tokens[index - 1].identifier);
					double _right = Convert.ToDouble(Tokens[index + 1].identifier);

					Token answer = new Token(TokenType.Number, "");
					answer.identifier = Math.Pow(_left, _right).ToString();

					Tokens.RemoveAt(index + 1);
					Tokens.RemoveAt(index);
					Tokens.RemoveAt(index - 1);
					Tokens.Insert(index - 1, answer);

					index = 0;			
				}

				if (Utils.isIndexValid(index + 1, Tokens))
					index++;
				else
					break;	
			} // can't do a traditional for loop because we are modifing the collection
			index = 0;

			// Order of operations: solve Multiply, Divide, and Modulus first
			while (true){
				Token token = Tokens[index];

				if ((token.type == TokenType.MultiplyOperator || token.type == TokenType.DivideOperator || token.type == TokenType.ModuloOperator) && Utils.isIndexValid(index - 1, Tokens) && Utils.isIndexValid(index + 1, Tokens)){
					Token _operator = Tokens[index];
					double _left = Convert.ToDouble(Tokens[index - 1].identifier);
					double _right = Convert.ToDouble(Tokens[index + 1].identifier);

					Token answer = new Token(TokenType.Number, "");
					if (_operator.type == TokenType.MultiplyOperator)
						answer.identifier = (_left * _right).ToString();
					else if (_operator.type == TokenType.DivideOperator){
						if (_right == 0)
							CowSpeak.FATAL_ERROR("Cannot divide by 0");
						answer.identifier = (_left / _right).ToString();
					}
					else if (_operator.type == TokenType.ModuloOperator)
						answer.identifier = (_left % _right).ToString();

					Tokens.RemoveAt(index + 1);
					Tokens.RemoveAt(index);
					Tokens.RemoveAt(index - 1);
					Tokens.Insert(index - 1, answer);

					index = 0;			
				}

				if (Utils.isIndexValid(index + 1, Tokens))
					index++;
				else
					break;	
			} // can't do a traditional for loop because we are modifing the collection
			index = 0;

			while (Tokens.Count > 1){
				Token token = Tokens[index];

				if (token.type != TokenType.Number && !Utils.isOperator(token.type))
					CowSpeak.FATAL_ERROR("Illegal token type '" + token.type.ToString() + "' in Evaluate");

				if (Utils.isIndexValid(index - 1, Tokens) && Utils.isIndexValid(index - 2, Tokens)){
					Token _operator = Tokens[index - 1];

					double _left = Convert.ToDouble(Tokens[index - 2].identifier);
					double _right = Convert.ToDouble(token.identifier);

					Token answer = new Token(TokenType.Number, "");
					if (_operator.type == TokenType.AddOperator)
						answer.identifier = (_left + _right).ToString();
					else if (_operator.type == TokenType.SubtractOperator)
						answer.identifier = (_left - _right).ToString();
					else
						CowSpeak.FATAL_ERROR("Cannot evaluate unknown token: " + answer.type.ToString());

					Tokens.RemoveAt(index);
					Tokens.RemoveAt(index - 1);
					Tokens.RemoveAt(index - 2);
					Tokens.Insert(0, answer);

					index = 0;
				}

				if (Utils.isIndexValid(index + 1, Tokens))
					index++;
				else
					break;
			}

			if (Tokens.Count > 1)
				CowSpeak.FATAL_ERROR("Could not evaluate expression");

			return Convert.ToDouble(Tokens[0].identifier);
		} 
	}
}