using CowSpeak.Exceptions;
using System.Collections.Generic;

namespace CowSpeak
{
	public class Functions : Dictionary<string, BaseFunction>
	{
		public Functions() : base()
		{

		}

		// parses a potential key so it can be directly used as a key (this[key])
		private string ParseKey(string key)
		{
			if (key.IndexOf("(") != -1)
				key = key.Substring(0, key.IndexOf("(")); // reduce the key to the text before the first '('

			if (key.IndexOf(".") != -1) // key is a method and we must reduce the member before the '.' to it's type
			{
				string caller = key.Substring(0, key.IndexOf("."));

				if (Interpreter.Vars.ContainsKey(caller)) // caller is an existing variable
				{
					Variable variable = Interpreter.Vars[caller];

					if (variable.Type.CSharpType != null && variable.Type.CSharpType.IsArray && ContainsKey("Array" + key.Substring(key.IndexOf("."))))
						key = "Array" + key.Substring(key.IndexOf("."));
					else
						key = variable.Type.Name + key.Substring(key.IndexOf("."));

					if (!ContainsKey(key)) // key is not a valid function, maybe it's an any method
					{
						string anyKey = "Any" + key.Substring(key.IndexOf("."));
						if (ContainsKey(anyKey)) // we're calling an any method, change the key
							key = anyKey;
					}
				}
				else if (Interpreter.Definitions.ContainsKey(caller))
				{
					caller = Interpreter.Definitions[caller].To;
					key = caller + key.Substring(key.IndexOf("."));
				}

				if (Type.GetType(caller, false) != null && !ContainsKey(key))
				{
					string anyKey = "Any" + key.Substring(key.IndexOf("."));
					if (ContainsKey(anyKey)) // we're calling an any method, change the key
						key = anyKey;
				}
			}

			return key;
		}

		public bool FunctionExists(string key) => ContainsKey(ParseKey(key));

		public new BaseFunction this[string key]
		{
			get
			{
				key = ParseKey(key);

				if (!ContainsKey(key))
					throw new BaseException("Function '" + key.Replace("\n", "\\n") + "' not found");

				return base[key];
			}
			set
			{
				this[key] = value;
			}
		}

		public BaseFunction Create(BaseFunction function)
		{
			if (ContainsKey(function.Name))
				throw new BaseException("Cannot create function '" + function.Name + "', a function by that name already exists");

			Add(function.Name, function);

			return function;
		}

		public void ClearUserDefined()
		{
			List<string> keysToRemove = new List<string>();

			foreach (var entry in this)
				if (entry.Value.DefinitionType == DefinitionType.User)
					keysToRemove.Add(entry.Key);

			keysToRemove.ForEach(key => Remove(key));
		}
	}
}