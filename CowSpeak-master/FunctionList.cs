using System.Collections.Generic;

namespace CowSpeak
{
	internal class FunctionList : List<FunctionBase>
	{
		public FunctionList() : base()
		{

		}

		internal FunctionBase Get(string functionName, bool _throw = true)
		{
			bool isValidMethodCall = false;
			if (functionName.IndexOf(".") != -1)
			{
				Variable obj = CowSpeak.Vars.Get(functionName.Substring(0, functionName.IndexOf(".")), false); // interpret variable name to the left of the period as a variable

				if (obj != null)
				{
					isValidMethodCall = true;
					if (obj.Type.rep.IsArray)
						functionName = "Array" + functionName.Substring(functionName.IndexOf("."));
					else
						functionName = obj.Type.Name + functionName.Substring(functionName.IndexOf("."));
				}
			} // if it has a period, it's probably a method

			if (isValidMethodCall || functionName.IndexOf(".") == -1 || Type.GetType(functionName.Substring(0, functionName.IndexOf(".")), false) == null)
			{
				for (int i = 0; i < Count; i++)
					if (functionName.IndexOf(base[i].Name) == 0)
						return base[i];
			}

			if (functionName.IndexOf(".") != -1)
			{
				string anyMethod = "Any" + functionName.Substring(functionName.IndexOf("."));

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