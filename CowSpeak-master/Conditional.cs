using System.Collections.Generic;

namespace CowSpeak{
	public class Conditional {
		public string text = "";
		
		public Conditional(string text){
			this.text = text.Substring(text.IndexOf("(") + 1, text.LastIndexOf(")") - text.IndexOf("(") - 1); // Reduce it to the text in between parenthesis
		}

		public bool EvaluateBoolean(){
			Token token = Lexer.ParseToken(text, false);

			string Evaluated = "";

			if (token == null)
				Evaluated = Evaluate.EvaluateBoolean(Lexer.ParseLine(text.Replace(((char)0x1D).ToString(), " "))).ToString();
			else{
				if (token.type == TokenType.VariableIdentifier)
					Evaluated = CowSpeak.getVariable(token.identifier).Get().ToString();
				else if (token.type == TokenType.FunctionCall)
					Evaluated = CowSpeak.findFunction(token.identifier).Execute(token.identifier).Get().ToString();
				else
					Evaluated = token.identifier;
			}

			return Evaluated == "True" || Evaluated == "1";
		}
	};

	public class RestrictedScope {
		public RestrictedScope(){
			// get a copy so it's not a reference
			oldVars = new List< Variable >(CowSpeak.Vars);
			oldDefs = new List< string[] >(CowSpeak.Definitions);
		} // any vars created in this scope will be destroyed at the end of the scope

		private List< Variable > oldVars = null;
		private List< string[] > oldDefs = null;

		public void End(){
			for (int i = 0; i < CowSpeak.Vars.Count; i++){
				bool matchFound = false;
				foreach (Variable oldVar in oldVars){
					if (oldVar.Name == CowSpeak.Vars[i].Name){
						matchFound = true;
						break;
					}
				}
				if (!matchFound)
					CowSpeak.Vars.RemoveAt(i); // was created in restricted scope because it didn't exist before the restricted scope began
			}

			for (int i = 0; i < CowSpeak.Definitions.Count; i++){
				bool matchFound = false;
				foreach (string[] oldDef in oldDefs){
					if (oldDef == CowSpeak.Definitions[i]){
						matchFound = true;
						break;
					}
				}
				if (!matchFound)
					CowSpeak.Vars.RemoveAt(i); // was created in restricted scope because it didn't exist before the restricted scope began
			}
		} // destroy any new vars & definitions created during the restricted scope
	}
}