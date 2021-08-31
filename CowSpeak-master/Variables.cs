using CowSpeak.Exceptions;
using System.Collections.Generic;
using System.Linq;

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

		public bool IsStructMemberAccessor(string key)
		{
			// there is only one "." in the key and it has at least one character to the left and to the right
			if (key.OccurrencesOf(".") == 1 && key.IndexOf(".") > 0 && key.IndexOf(".") < key.Length - 1)
			{
				string leftOf = key.Substring(0, key.IndexOf("."));
				string rightOf = key.Substring(key.IndexOf(".") + 1);
				// leftOf is a valid variable, it's type is a FastStruct, and that FastStruct has rightOf as one of it's members
				if (base.ContainsKey(leftOf) && this[leftOf].Type is FastStruct && (this[leftOf].Type as FastStruct).Members.ContainsKey(rightOf))
				{
					return true;
				}
			}

			return false;
		}

		// we must overload this function in order to add support for calling members of types
		public new bool ContainsKey(string key) => base.ContainsKey(key);// || IsStructMemberAccessor(key);

		public new Variable this[string key]
		{
			get
			{
				if (base.ContainsKey(key))
				{
					return base[key];
				}

				throw new BaseException("Could not find variable: " + key);
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