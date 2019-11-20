using System.Collections.Generic;
using System;

namespace CowSpeak{
	public static class Evaluate{
		public static List< Token > EvaluateMD(List< Token > Tokens){
			int index = 0;
			while (true){
				Token token = Tokens[index];

				if ((token.type == TokenType.MultiplyOperator || token.type == TokenType.DivideOperator || token.type == TokenType.ModuloOperator) && Utils.IsIndexValid(index - 1, Tokens) && Utils.IsIndexValid(index + 1, Tokens)){
					Token _operator = Tokens[index];
					double _left = Convert.ToDouble(Tokens[index - 1].identifier);
					double _right = Convert.ToDouble(Tokens[index + 1].identifier);

					Token answer = new Token(TokenType.Number, "");
					if (_operator.type == TokenType.MultiplyOperator)
						answer.identifier = (_left * _right).ToString();
					else if (_operator.type == TokenType.DivideOperator){
						if (_right == 0)
							CowSpeak.FatalError("Cannot divide by 0");
						answer.identifier = (_left / _right).ToString();
					}
					else if (_operator.type == TokenType.ModuloOperator){
						if (_right == 0)
							CowSpeak.FatalError("Cannot divide by 0");
						answer.identifier = (_left % _right).ToString();
					}

					Tokens.RemoveAt(index + 1);
					Tokens.RemoveAt(index);
					Tokens.RemoveAt(index - 1);
					Tokens.Insert(index - 1, answer);

					index = 0;			
				}

				if (Utils.IsIndexValid(index + 1, Tokens))
					index++;
				else
					break;	
			} // can't do a traditional for loop because we are modifing the collection
			return Tokens;
		}

		public static double EvaluateTokens(List< Token > Tokens){
			int index = 0;

			try{
				// Order of operations: solve exponents first
				while (true){
					Token token = Tokens[index];

					if (token.type == TokenType.PowerOperator && Utils.IsIndexValid(index - 1, Tokens) && Utils.IsIndexValid(index + 1, Tokens)){
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

					if (Utils.IsIndexValid(index + 1, Tokens))
						index++;
					else
						break;	
				} // can't do a traditional for loop because we are modifing the collection
				index = 0;

				// Order of operations: solve Multiply, Divide, and Modulus first
				Tokens = EvaluateMD(Tokens);

				while (Tokens.Count > 1){
					Token token = Tokens[index];

					if (token.type != TokenType.Number && !Utils.IsOperator(token.type))
						CowSpeak.FatalError("Illegal token type '" + token.type.ToString() + "' in EvaluateTokens");

					if (Utils.IsIndexValid(index - 1, Tokens) && Utils.IsIndexValid(index - 2, Tokens)){
						Token _operator = Tokens[index - 1];

						double _left = Convert.ToDouble(Tokens[index - 2].identifier);
						double _right = Convert.ToDouble(token.identifier);

						Token answer = new Token(TokenType.Number, "");
						if (_operator.type == TokenType.AddOperator)
							answer.identifier = (_left + _right).ToString();
						else if (_operator.type == TokenType.SubtractOperator)
							answer.identifier = (_left - _right).ToString();
						else
							CowSpeak.FatalError("Cannot evaluate non-operator token: " + answer.type.ToString());

						Tokens.RemoveAt(index);
						Tokens.RemoveAt(index - 1);
						Tokens.RemoveAt(index - 2);
						Tokens.Insert(0, answer);

						index = 0;
					}

					if (Utils.IsIndexValid(index + 1, Tokens))
						index++;
					else
						break;
				}

				if (Tokens.Count > 1){
					string errMsg = "Could not evaluate expression: '";
					foreach (Token token in Tokens){
						errMsg += token.identifier + " ";
					}
					errMsg += "'";
					CowSpeak.FatalError(errMsg);
				}

				return Convert.ToDouble(Tokens[0].identifier);
			}
			catch (Exception ex) {
				string errMsg = "Could not evaluate expression: '";
				foreach (Token token in Tokens){
					errMsg += token.identifier + " ";
				}
				errMsg += "'\nError: " + ex.Message;
				CowSpeak.FatalError(errMsg);
				return 0;
			}
		} 

		public static bool EvaluateBoolean(List< Token > Tokens){
			int index = 0;

			try{
				while (Tokens.Count > 1){
					Token token = Tokens[index];

					if (Utils.IsIndexValid(index - 1, Tokens) && Utils.IsIndexValid(index + 1, Tokens) && token.type.ToString().IndexOf("Is") != -1){
						Token _operator = token;

						Any _left = new TokenLine(new List< Token >{ Tokens[index - 1] }).Exec();
						Any _right = new TokenLine(new List< Token >{ Tokens[index + 1] }).Exec();

						Token answer = new Token(TokenType.Number, "");
						if (_operator.type == TokenType.IsEqualOperator)
							answer.identifier = (_left.Get().ToString() == _right.Get().ToString()).ToString();
						else if (_operator.type == TokenType.IsNotEqualOperator)
							answer.identifier = (_left.Get().ToString() != _right.Get().ToString()).ToString();
						else if (_operator.type == TokenType.IsGreaterThanOperator || _operator.type == TokenType.IsLessThanOperator){
							if (!Utils.IsDigitsOnly(_left.Get().ToString()) || !Utils.IsDigitsOnly(_right.Get().ToString()))
								CowSpeak.FatalError("Cannot perform '" + _operator.type.ToString() + "' with non-number operands");

							if (_operator.type == TokenType.IsGreaterThanOperator)
								answer.identifier = (Convert.ToDouble(_left.Get()) > Convert.ToDouble(_right.Get())).ToString();
							else if (_operator.type == TokenType.IsLessThanOperator)
								answer.identifier = (Convert.ToDouble(_left.Get()) < Convert.ToDouble(_right.Get())).ToString();
						}
						else
							CowSpeak.FatalError("Cannot evaluate non-comparison token: " + answer.type.ToString());

						answer.identifier = Utils.FixBoolean(answer.identifier);

						Tokens.RemoveAt(index + 1);
						Tokens.RemoveAt(index);
						Tokens.RemoveAt(index - 1);
						Tokens.Insert(0, answer);

						index = 0;
					}

					if (Utils.IsIndexValid(index + 1, Tokens))
						index++;
					else
						break;
				}

				if (Tokens.Count > 1){
					string errMsg = "Could not evaluate expression: '";
					foreach (Token token in Tokens)
						errMsg += token.identifier + " ";
					errMsg += "'";
					CowSpeak.FatalError(errMsg);
				}

				return Tokens[0].identifier == "1" || Tokens[0].identifier == "True";
			}
			catch (Exception ex) {
				string errMsg = "Could not evaluate expression: '";
				foreach (Token token in Tokens)
					errMsg += token.identifier + " ";
				errMsg += "'\nError: " + ex.Message;
				CowSpeak.FatalError(errMsg);
				return false;
			}
		} 
	}
}