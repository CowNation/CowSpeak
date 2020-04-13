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
			ExpressionTokens = Lexer.ParseLine(text.Substring(text.IndexOf("(") + 1, text.LastIndexOf(")") - text.IndexOf("(") - 1).Replace(((char)0x1D).ToString(), " ")); // Reduce it to the text in between parenthesis and parse
		}

		public bool EvaluateExpression()
		{
			Any AlreadyEvaluatedValue = null;
			string Expression = Utils.GetTokensExpression(ExpressionTokens, ref AlreadyEvaluatedValue);
			if (AlreadyEvaluatedValue != null)
				return AlreadyEvaluatedValue.ToString() == "true" || AlreadyEvaluatedValue.ToString() == "1";

			object Interpreted = Utils.Eval(Expression);
			return Interpreted.ToString() == "true" || Interpreted.ToString() == "1";
		}
	}
}