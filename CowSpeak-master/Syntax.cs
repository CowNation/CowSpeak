namespace CowSpeak
{
	namespace Syntax
	{
		public static class Types
		{
			public const string Void = "void";
			public const string c_Void = "Void";
			public const string Decimal = "decimal";
			public const string c_Decimal = "Decimal";
			public const string Integer = "integer";
			public const string c_Integer = "Integer";
			public const string Integer64 = "integer64";
			public const string c_Integer64 = "Integer64";
			public const string String = "string";
			public const string c_String = "String";
			public const string Boolean = "boolean";
			public const string c_Boolean = "Boolean";
			public const string Character = "character";
			public const string c_Character = "Character";
			public const string Any = "object";
			public const string c_Any = "Object";
			public const string ArraySuffix = "Array";
		}

		public static class Statements
		{
			public const string Return = "return";
		}

		public static class Conditionals
		{
			public const string If = "if";
			public const string Else = "else";
			public const string While = "while";
			public const string Loop = "loop";
		}

		public static class Identifiers
		{
			public const string Comment = "#";
		}

		public static class Comparators
		{
			public const string IsEqual = "==";
			public const string IsNotEqual = "!=";
			public const string IsGreaterThan = ">";
			public const string IsLessThan = "<";
			public const string IsGreaterThanOrEqual = ">=";
			public const string IsLessThanOrEqual = "<=";
		}

		public static class Operators
		{
			public const string Add = "+";
			public const string Subtract = "-";
			public const string Multiply = "*";
			public const string Divide = "/";
			public const string Modulo = "%";
			public const string Equal = "=";
			public const string And = "&&";
			public const string Or = "||";
			public const string BitwiseAND = "&";
			public const string BitwiseOR = "|";
		}
	}
}