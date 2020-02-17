using System.Collections.Generic;
using System.Data;

namespace CowSpeak
{
	public static class Evaluate
	{
		public static double EvaluateExpression(string expression)
		{
			try
			{
				return System.Convert.ToDouble(new DataTable().Compute(expression, null));
			}
			catch (System.Data.EvaluateException ex)
			{
				throw new Exception("Could not evaluate expression: " + ex.Message);
			}
		}

		public static bool EvaluateBoolean(List< Token > Tokens)
		{
			int index = 0;

			while (Tokens.Count > 1)
			{
				Token token = Tokens[index];

				if (Tokens.IsIndexValid(index - 1) && Tokens.IsIndexValid(index + 1) && token.type.ToString().IndexOf("Is") != -1)
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

				if (Tokens.IsIndexValid(index + 1))
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