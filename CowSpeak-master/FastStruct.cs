using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace CowSpeak
{
    public class FastStruct : Type
    {
		public Dictionary<string, Parameter> Members = new Dictionary<string, Parameter>();

		public FastStruct(string name, Parameter[] members) : base(name, null)
		{
			foreach (var member in members)
				Members.Add(member.Name, member);
		} 

		public static Parameter[] ParseDefinitionParams(string s_params) => UserFunction.ParseDefinitionParams(s_params);

		public static bool IsFastStruct(string token)
		{
			if (token.IndexOf("<") == -1 || Utils.GetClosingBracket(token) != token.Length - 1)
				return false;

			return Utils.IsValidObjectName(token.Substring(0, token.IndexOf("<")));
		}

		public static bool IsValidMemberAccessor(string token)
		{
			// there is only one "." in the key and it has at least one character to the left and to the right
			if (token.OccurrencesOf(".") == 1 && token.IndexOf(".") > 0 && token.IndexOf(".") < token.Length - 1)
			{
				string leftOf = token.Substring(0, token.IndexOf("."));
				string rightOf = token.Substring(token.IndexOf(".") + 1);
				return Utils.IsValidObjectName(leftOf) && Utils.IsValidObjectName(rightOf);
			}

			return false;
		}
	}
}
