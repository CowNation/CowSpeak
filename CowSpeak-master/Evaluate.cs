using System.Collections.Generic;

namespace CowSpeak
{
	public static class Evaluate
	{
		public static List< Token > EvaluateMD(List< Token > Tokens)
		{
			int index = 0;
			while (true)
			{
				Token token = Tokens[index];

				if ((token.type == TokenType.MultiplyOperator || token.type == TokenType.DivideOperator || token.type == TokenType.ModuloOperator) && Utils.IsIndexValid(index - 1, Tokens) && Utils.IsIndexValid(index + 1, Tokens)){
					Token _operator = Tokens[index];
					double _left = System.Convert.ToDouble(Tokens[index - 1].identifier);
					double _right = System.Convert.ToDouble(Tokens[index + 1].identifier);

					Token answer = new Token(TokenType.Number, "");
					if (_operator.type == TokenType.MultiplyOperator)
						answer.identifier = (_left * _right).ToString();
					else if (_operator.type == TokenType.DivideOperator)
					{
						if (_right == 0)
							throw new Exception("Cannot divide by 0");
						answer.identifier = (_left / _right).ToString();
					}
					else if (_operator.type == TokenType.ModuloOperator)
					{
						if (_right == 0)
							throw new Exception("Cannot divide by 0");
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

		public static double EvaluateTokens(List< Token > Tokens)
		{
			int index = 0;

			// Order of operations: solve exponents first
			while (true)
			{
				Token token = Tokens[index];

				if (token.type == TokenType.PowerOperator && Utils.IsIndexValid(index - 1, Tokens) && Utils.IsIndexValid(index + 1, Tokens))
				{
					Token _operator = Tokens[index];
					double _left = System.Convert.ToDouble(Tokens[index - 1].identifier);
					double _right = System.Convert.ToDouble(Tokens[index + 1].identifier);

					Token answer = new Token(TokenType.Number, "");
					answer.identifier = System.Math.Pow(_left, _right).ToString();

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

			while (Tokens.Count > 1)
			{
				Token token = Tokens[index];

				if (token.type != TokenType.Number && !Utils.IsOperator(token.type))
					throw new Exception("Illegal token type '" + token.type.ToString() + "' in EvaluateTokens");

				if (Utils.IsIndexValid(index - 1, Tokens) && Utils.IsIndexValid(index - 2, Tokens))
				{
					
					Token _operator = Tokens[index - 1];

					double _left = System.Convert.ToDouble(Tokens[index - 2].identifier);
					double _right = System.Convert.ToDouble(token.identifier);

					Token answer = new Token(TokenType.Number, "");
					if (_operator.type == TokenType.AddOperator)
						answer.identifier = (_left + _right).ToString();
					else if (_operator.type == TokenType.SubtractOperator)
						answer.identifier = (_left - _right).ToString();
					else
						throw new Exception("Cannot evaluate non-operator token: " + _operator.type.ToString() + " " + _operator.identifier.ToString());

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

			if (Tokens.Count > 1)
			{
				string errMsg = "Could not evaluate expression: '";
				foreach (Token token in Tokens)
					errMsg += token.identifier + " ";
				errMsg += "'";
				throw new Exception(errMsg);
			}

			return System.Convert.ToDouble(Tokens[0].identifier);
		}

		public static bool EvaluateBoolean(List< Token > Tokens)
		{
			int index = 0;

			while (Tokens.Count > 1)
			{
				Token token = Tokens[index];

				if (Utils.IsIndexValid(index - 1, Tokens) && Utils.IsIndexValid(index + 1, Tokens) && token.type.ToString().IndexOf("Is") != -1)
				{
					Token _operator = token;

					Any _left = new Line(new List< Token >{ Tokens[index - 1] }).Exec();
					Any _right = new Line(new List< Token >{ Tokens[index + 1] }).Exec();
					if (!Conversion.IsCompatible(_left.vType, _right.vType))
						throw new Exception(_left.vType.Name + " is incompatible with " + _right.vType.Name);

					Token answer = new Token(TokenType.Number, "");
					if (_operator.type == TokenType.IsEqualOperator)
						answer.identifier = (_left.Get().ToString() == _right.Get().ToString()).ToString();
					else if (_operator.type == TokenType.IsNotEqualOperator)
						answer.identifier = (_left.Get().ToString() != _right.Get().ToString()).ToString();
					else if (_operator.type == TokenType.IsGreaterThanOperator || _operator.type == TokenType.IsLessThanOperator || _operator.type == TokenType.IsGreaterThanOrEqualOperator || _operator.type == TokenType.IsLessThanOrEqualOperator)
					{
						if (!Utils.IsDigitsOnly(_left.Get().ToString()) || !Utils.IsDigitsOnly(_right.Get().ToString()))
							throw new Exception("Cannot perform '" + _operator.type.ToString() + "' with non-number operands");

						if (_operator.type == TokenType.IsGreaterThanOperator)
							answer.identifier = (System.Convert.ToDouble(_left.Get()) > System.Convert.ToDouble(_right.Get())).ToString();
						else if (_operator.type == TokenType.IsLessThanOperator)
							answer.identifier = (System.Convert.ToDouble(_left.Get()) < System.Convert.ToDouble(_right.Get())).ToString();
						else if (_operator.type == TokenType.IsGreaterThanOrEqualOperator)
							answer.identifier = (System.Convert.ToDouble(_left.Get()) >= System.Convert.ToDouble(_right.Get())).ToString();
						else if (_operator.type == TokenType.IsLessThanOrEqualOperator)
							answer.identifier = (System.Convert.ToDouble(_left.Get()) <= System.Convert.ToDouble(_right.Get())).ToString();
					}
					else
						throw new Exception("Cannot evaluate non-comparison token: " + answer.type.ToString());

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

			if (Tokens.Count > 1)
			{
				string errMsg = "Could not evaluate expression: '";
				foreach (Token token in Tokens)
					errMsg += token.identifier + " ";
				errMsg += "'";
				throw new Exception(errMsg);
			}

			return Tokens[0].identifier == "1" || Tokens[0].identifier == "True";
		} 
	}
}