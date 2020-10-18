using CowSpeak.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CowSpeak
{
	public class Type
	{
		// The name that this type will be referred to as
		public string Name;

		// The C# type representation for this type
		public System.Type representation;

		// Can the type be initialized as an array
		public bool CanBeArray;

		public Type()
		{

		}

		public Type(string name, System.Type representation, bool canBeArray = true)
		{
			this.Name = name;
			this.representation = representation;
			this.CanBeArray = canBeArray;
		}

		public Type MakeArrayType()
		{
			if (!CanBeArray)
			{
				throw new BaseException("The type '" + Name + "' cannot be initialized as an array");
			}
			
			return new Type(char.ToUpper(Name[0]) + Name.Substring(1) + Syntax.Types.ArraySuffix, representation.MakeArrayType());
		}

		public static Type Object = new Type(Syntax.Types.Object, typeof(object));
		public static Type Void = new Type(Syntax.Types.Void, typeof(void), false);
		public static Type Integer = new Type(Syntax.Types.Integer, typeof(int));
		public static Type Integer64 = new Type(Syntax.Types.Integer64, typeof(long));
		public static Type Decimal = new Type(Syntax.Types.Decimal, typeof(double));
		public static Type String = new Type(Syntax.Types.String, typeof(string));
		public static Type Boolean = new Type(Syntax.Types.Boolean, typeof(bool));
		public static Type Character = new Type(Syntax.Types.Character, typeof(char));
		public static Type Byte = new Type(Syntax.Types.Byte, typeof(byte));

		public static Type ObjectArray = Object.MakeArrayType();
		public static Type IntegerArray = Integer.MakeArrayType();
		public static Type Integer64Array = Integer64.MakeArrayType();
		public static Type DecimalArray = Decimal.MakeArrayType();
		public static Type StringArray = String.MakeArrayType();
		public static Type BooleanArray = Boolean.MakeArrayType();
		public static Type CharacterArray = Character.MakeArrayType();
		public static Type ByteArray = Byte.MakeArrayType();
		
		public static readonly Type[] Types = typeof(Type)
					.GetFields(BindingFlags.Public | BindingFlags.Static)
					.Where(member => member.FieldType == typeof(Type))
					.Select(member => (Type)member.GetValue(null))
					.ToArray();

		public static Type GetType(System.Type rep, bool _throw = true)
		{
			foreach (Type type in Types)
			{
				if (type.representation == rep)
					return type;
			}
			
			if (_throw)
				throw new BaseException("Cannot determine type from representation: " + rep.Name);
			return null;
		}

		public static Type GetType(string typeName, bool _throw = true)
		{
			if (Interpreter.Definitions.ContainsKey(typeName))
				typeName = Interpreter.Definitions[typeName].To;

			foreach (Type type in Types)
				if (type.Name == typeName)
					return type;

			if (_throw)
				throw new BaseException("Type '" + typeName + "' does not exist");

			return null;
		}
	}
}