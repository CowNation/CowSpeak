using System.Collections.Generic;
using System.Linq;
using DynamicExpresso;

namespace CowSpeak
{
	internal class Conditional
	{
		public List<Token> ExpressionTokens = new List<Token>();
		
		public Conditional(string text)
		{
			ExpressionTokens = Lexer.ParseLine(text.Substring(text.IndexOf("(") + 1, text.LastIndexOf(")") - text.IndexOf("(") - 1)); // Reduce it to the text in between parenthesis and parse
		}

		public bool EvaluateExpression()
		{
			Any alreadyEvaluatedValue = null;
			string expression = Utils.GetTokensExpression(ExpressionTokens, ref alreadyEvaluatedValue);
			if (alreadyEvaluatedValue != null)
				return alreadyEvaluatedValue.ToString() == "True" || alreadyEvaluatedValue.ToString() == "1";

			object interpreted = Utils.Eval(expression);
			return interpreted.ToString() == "True" || interpreted.ToString() == "1";
		}
	}
}