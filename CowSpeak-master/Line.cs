using System.Collections.Generic;
using CowSpeak.Exceptions;
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
					if (Interpreter.Functions[base[i].identifier].ReturnType == Type.Void)
					{
						if (GetRange(0, Count).IsIndexValid(i - 1) && Utils.IsOperator(base[i - 1].type))
							throw new BaseException("Cannot perform operation: '" + base[i - 1].identifier + "' on void function");
						if (GetRange(0, Count).IsIndexValid(i + 1) && Utils.IsOperator(base[i + 1].type))
							throw new BaseException("Cannot perform operation: '" + base[i + 1].identifier + "' on void function");
					}
				}
			}

			for (int i = 0; i < Count; i++)
			{
				if (base[i].type == TokenType.EqualOperator)
				{
					if (i > 0 && base[i - 1].type != TokenType.VariableIdentifier)
						throw new BaseException("Cannot assign to a " + base[i - 1].type.ToString() + ", it's a non-modifiable token");

					toEval = GetRange(i + 1, Count - (i + 1));
					break;
				}
			} // remove the equal sign and everything to the left from the equation

			if (Count == 2 && base[0].type == TokenType.TypeIdentifier && base[1].type == TokenType.VariableIdentifier)
				return null; // support for uninitialized vars

			Any alreadyEvaluatedValue = null;
			string expression = Utils.GetTokensExpression(toEval, ref alreadyEvaluatedValue);
			if (alreadyEvaluatedValue != null)
				return alreadyEvaluatedValue; // expression was already evaluated previously

			if (expression.Length == 0)
				return null;

			var evaluated = Utils.Eval(expression);

			if (evaluated.GetType() == typeof(uint)) // causes exception sometimes
				return new Any(Type.Integer64, evaluated);

			return new Any(evaluated);
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