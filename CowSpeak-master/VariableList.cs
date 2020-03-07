using System.Collections.Generic;

namespace CowSpeak
{
	internal class VariableList : List<Variable>
	{
		public VariableList() : base()
		{

		}

		public void Create(Variable variable)
		{
			if (Get(variable.Name, false) != null) // already exists
				throw new Exception("Cannot create variable '" + variable.Name + "', a variable by that name already exists");
			
			Add(variable);
		}

		public Variable Get(string varName, bool _throw = true)
		{
			for (int i = 0; i < Count; i++)
				if (base[i].Name == varName)
					return base[i];

			if (_throw)
				throw new Exception("Could not find variable: " + varName);

			return null;
		}
	}
}