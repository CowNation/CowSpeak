using System.Collections.Generic;
using System.Linq;

namespace CowSpeak
{
	internal class Scope
	{
		public Scope()
		{
			// get a copy so it's not a reference
			oldVars = new List<string>(Interpreter.Vars.Keys);
		} // any vars created in this scope will be destroyed at the end of the scope

		public List<string> oldVars = null;

		public void End()
		{
			List<string> keysToRemove = new List<string>(); // a list of variable names to remove after checking for new vars
			foreach (var entry in Interpreter.Vars)
			{
				if (oldVars.Where(x => x == entry.Key).FirstOrDefault() == null)
				{
					keysToRemove.Add(entry.Key);
				}
			}
			keysToRemove.ForEach(key => Interpreter.Vars.Remove(key));
		}
	}
}