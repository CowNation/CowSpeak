using System.Collections.Generic;
using DynamicExpresso;

namespace CowSpeak
{
	public class Line : List< Token >
	{
		public Line(List< Token > tokens) : base(tokens)
		{

		}

		public bool HasFunctionDefinition
		{
			get
			{
				return Count > 1 && base[0].type == TokenType.TypeIdentifier && base[1].type == TokenType.FunctionCall;
			}
		}

		public bool HasConditional
		{
			get
			{
				return Count > 0 && base[0].type.ToString().IndexOf("Conditional") != -1;
			}
		}

		public Any Exec()
		{
			List< Token > toEval = GetRange(0, Count); // get a copy

			for (int i = 0; i < Count; i++)
			{
				if (base[i].type == TokenType.FunctionCall)
				{
					if (CowSpeak.Functions.Get(base[i].identifier).type == Type.Void){
						if (GetRange(0, Count).IsIndexValid(i - 1) && Utils.IsOperator(base[i-1].type))
							throw new Exception("Cannot perform operation: '" + base[i-1].identifier + "' on void function");
						if (GetRange(0, Count).IsIndexValid(i + 1) && Utils.IsOperator(base[i+1].type))
							throw new Exception("Cannot perform operation: '" + base[i+1].identifier + "' on void function");
					}
				}
			}

			for (int i = 0; i < Count; i++)
			{
				if (base[i].type == TokenType.EqualOperator)
				{
					toEval = GetRange(i + 1, Count - (i + 1));
					break;
				}
			} // remove the equal sign and everything to the left

			if (Count == 2 && base[0].type == TokenType.TypeIdentifier && base[1].type == TokenType.VariableIdentifier)
				return null; // support for uninitialized vars

			Any AlreadyEvaluatedValue = null;
			string Expression = Utils.GetTokensExpression(toEval, ref AlreadyEvaluatedValue);
			if (AlreadyEvaluatedValue != null)
				return AlreadyEvaluatedValue;

			if (Expression.Length == 0)
				return new Any(Type.Integer, 0);

			if (Expression.OccurrencesOf(" ") == 0 && Utils.IsNumber(Expression))
			{
				Any val = new Any();
				if (Expression.IndexOf(".") != -1)
				{
					val.Type = Type.Decimal;
					val.Value = double.Parse(Expression);
				}
				else
				{
					val.Type = Type.Integer64;
					val.Value = long.Parse(Expression);
					if ((long)val.Value < int.MaxValue)
						val.Type = Type.Integer;
				}
				
				return val;
			}

			return new Any(Utils.Eval(Expression));
		}

		public override string ToString()
		{
			string ret = "";
			for (int i = 0; i < Count; i++)
			{
				ret += base[i].identifier + (i < Count - 1 ? " " : "");
			}
			return ret;
		}
	}
}