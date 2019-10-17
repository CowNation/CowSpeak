using System.Collections.Generic;

namespace CowSpeak{
	public class Conditional {
		public string text = "";
		
		public Conditional(string text){
			this.text = text.Substring(text.IndexOf("(") + 1, text.LastIndexOf(")") - text.IndexOf("(") - 1); // Reduce it to the text in between parenthesis
		}

		public bool Evaluate(){
			Token token = Lexer.ParseToken(text);

			string evaluation = "";

			if (token.type == TokenType.FunctionCall){
				Function func = CowSpeak.findFunction(token.identifier);
				evaluation = func.Execute(token.identifier).Get().ToString();
			}
			else if (token.type == TokenType.VariableIdentifier){
				Variable _var = CowSpeak.getVariable(token.identifier);
				evaluation = _var.Get().ToString();
			}
			else
				evaluation = token.identifier;

			return evaluation == "1" || evaluation == "True"; // is true
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