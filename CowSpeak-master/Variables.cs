using System.Collections.Generic;

namespace CowSpeak
{
	internal class Variables : Dictionary<string, Variable>
	{
		public Variables() : base()
		{

		}

		public new Variable this[string key]
		{
			get
			{
				if (!ContainsKey(key))
					throw new Exception("Could not find variable: " + key);

				return base[key];
			}
			set
			{
				this[key] = value;
			}
		}

		public Variable Create(Variable variable)
		{
			if (ContainsKey(variable.Name))
				throw new Exception("Cannot create variable '" + variable.Name + "', a variable by that name already exists");
			
			Add(variable.Name, variable);
			
			return variable;
		}
	}
}