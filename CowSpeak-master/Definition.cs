using System;

namespace CowSpeak
{
	// This pertains to functions and definitions
	internal enum DefinitionType
	{
		Static,
		User
	}

	public struct Definition
	{
		public string From;
		public string To;
		internal DefinitionType DefinitionType;
	
		// Any enums inside of a module with this attribute's elements will be considered a definition; From is the name of the element and To is the value of the element
		[AttributeUsage(AttributeTargets.Enum)]
		public class EnumAttribute : Attribute
		{

		}
	}
}