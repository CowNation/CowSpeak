using System.Collections.Generic;

namespace CowSpeak
{
	internal class FunctionList : List<FunctionBase>
	{
		public FunctionList() : base()
		{

		}

		internal FunctionBase Get(string functionName, bool _throw = true, bool AcceptStaticMethods = false)
		{
			bool isValidMethodCall = false;
			string Name = functionName.IndexOf("(") != -1 ? functionName.Substring(0, functionName.IndexOf("(")) : functionName;
			if (Name.IndexOf(".") != -1)
			{
				Variable obj = CowSpeak.Vars.Get(Name.Substring(0, Name.IndexOf(".")), false); // interpret variable name to the left of the period as a variable

				if (obj != null)
				{
					isValidMethodCall = true;
					if (obj.Type.rep.IsArray)
						Name = "Array" + Name.Substring(Name.IndexOf("."));
					else
						Name = obj.Type.Name + Name.Substring(Name.IndexOf("."));
				}
				else if (AcceptStaticMethods && Type.GetType(Name.Substring(0, Name.IndexOf(".")), false) != null) // ex: string.IndexOf
					isValidMethodCall = true;
			} // if it has a period, it's probably a method

			if (isValidMethodCall || Name.IndexOf(".") == -1)
			{
				for (int i = 0; i < Count; i++)
					if (Name.IndexOf(base[i].Name) == 0)
						return base[i];
			}

			if (Name.IndexOf(".") != -1)
			{
				string anyMethod = "Any" + Name.Substring(Name.IndexOf("."));

				for (int i = 0; i < Count; i++)
					if (anyMethod.IndexOf(base[i].Name) == 0)
						return base[i];
			} // try to see if it's an 'Any' method

			if (_throw)
				throw new Exception("Function '" + functionName.Replace(System.Environment.NewLine, "\\n") + "' not found");

			return null;
		}

		internal void Create(FunctionBase func)
		{
			if (Get(func.Name, false) != null) // already exists
				throw new Exception("Cannot create function '" + func.Name + "', a function by that name already exists");
			
			Add(func);
		}

		internal void ClearUserDefined()
		{
			for (int i = 0; i < Count; i++)
				if (base[i].DefinitionType == DefinitionType.User)
					RemoveAt(i);
		}
	}
}