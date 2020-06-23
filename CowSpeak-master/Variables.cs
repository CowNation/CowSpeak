using CowSpeak.Exceptions;
using System.Collections.Generic;

namespace CowSpeak
{
	internal class Variables : Dictionary<string, Variable>
	{
		public Variables() : base()
		{

		}

		public new void Add(string key, Variable value)
		{
			if (ContainsKey(key))
				throw new BaseException("Variable '" + key + "' has already been defined");

			base.Add(key, value);
		}

		public new Variable this[string key]
		{
			get
			{
				if (!ContainsKey(key))
					throw new BaseException("Could not find variable: " + key);

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
				throw new BaseException("Cannot create variable '" + variable.Name + "', a variable by that name already exists");
			
			Add(variable.Name, variable);
			
			return variable;
		}
	}
}