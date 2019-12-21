using System.Collections.Generic;
using System.Linq;

namespace CowSpeak{
	public class Conditional {
		public string text = "";
		
		public Conditional(string text){
			this.text = text.Substring(text.IndexOf("(") + 1, text.LastIndexOf(")") - text.IndexOf("(") - 1); // Reduce it to the text in between parenthesis
		}

		public bool EvaluateBoolean(){
			List< string > Expressions = text.Split(Syntax.Operators.And).ToList();
			bool Evaluated = true;

			foreach (string Expression in Expressions){
				bool _Evaluated = false;
				Token token = Lexer.ParseToken(text, false);
				if (token == null)
					_Evaluated = Evaluate.EvaluateBoolean(Lexer.ParseLine(Expression.Replace(((char)0x1D).ToString(), " ")));
				else{
					string simplified = "";
					if (token.type == TokenType.VariableIdentifier)
						simplified = CowSpeak.GetVariable(token.identifier).Get().ToString();
					else if (token.type == TokenType.FunctionCall)
						simplified = CowSpeak.GetFunction(token.identifier).Execute(token.identifier).Get().ToString();
					else
						simplified = token.identifier;
					_Evaluated = simplified == "True" || simplified == "1";
				}
				if (!_Evaluated)
					Evaluated = false;
			}

			return Evaluated;
		}
	};
}