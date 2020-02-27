using System.Collections.Generic;

namespace CowSpeak
{
	internal class Scope
	{
		public Scope()
		{
			// get a copy so it's not a reference
			oldVars = new List< Variable >(CowSpeak.Vars);
			oldDefs = new List< Definition >(CowSpeak.Definitions);
		} // any vars created in this scope will be destroyed at the end of the scope

		public List< Variable > oldVars = null;
		public List< Definition > oldDefs = null;

		public void End()
		{
			for (int i = 0; i < CowSpeak.Vars.Count; i++)
			{
				bool matchFound = false;
				foreach (Variable oldVar in oldVars)
				{
					if (oldVar.Name == CowSpeak.Vars[i].Name)
					{
						matchFound = true;
						break;
					}
				}
				if (!matchFound)
				{
					CowSpeak.Vars.RemoveAt(i); // was created in restricted scope because it didn't exist before the restricted scope began
					
					if (i > 0)
						i--;
				}

				if (i < 0 || i >= CowSpeak.Vars.Count)
					break;
			}

			for (int i = 0; i < CowSpeak.Definitions.Count; i++)
			{
				bool matchFound = false;
				foreach (Definition oldDef in oldDefs)
				{
					if (oldDef.from == CowSpeak.Definitions[i].from && oldDef.to == CowSpeak.Definitions[i].to){
						matchFound = true;
						break;
					}
				}
				if (!matchFound && CowSpeak.Definitions[i].DefinitionType != DefinitionType.Language){
					CowSpeak.Vars.RemoveAt(i); // was created in restricted scope because it didn't exist before the restricted scope began

					if (i > 0)
						i--;				
				}

				if (i < 0 || i >= CowSpeak.Vars.Count)
					break;
			}
		} // destroy any new vars & definitions created during the restricted scope
	}
}