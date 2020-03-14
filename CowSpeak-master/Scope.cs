using System.Collections.Generic;

namespace CowSpeak
{
	internal class Scope
	{
		public Scope()
		{
			// get a copy so it's not a reference
			oldVars = new List< Variable >(CowSpeak.Vars);
		} // any vars created in this scope will be destroyed at the end of the scope

		public List< Variable > oldVars = null;

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
					
					i--;
				}

				if (!CowSpeak.Vars.IsIndexValid(i) && i >= 0)
					break;
			} // destroy any new vars created during the restricted scope
		}
	}
}