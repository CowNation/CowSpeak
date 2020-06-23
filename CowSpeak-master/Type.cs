using CowSpeak.Exceptions;
using System.Linq;
using System.Reflection;

namespace CowSpeak
{
	public class Type
	{
		public string Name; // how the type is referenced in the code
		public System.Type rep; // C# representation of type

		public Type(string Name, System.Type rep)
		{
			this.Name = Name;
			this.rep = rep;
		}

		public static Type Any = new Type(Syntax.Types.Any, typeof(object));
		public static Type Void = new Type(Syntax.Types.Void, typeof(void));
		public static Type Integer = new Type(Syntax.Types.Integer, typeof(int));
		public static Type Integer64 = new Type(Syntax.Types.Integer64, typeof(long));
		public static Type Decimal = new Type(Syntax.Types.Decimal, typeof(double));
		public static Type String = new Type(Syntax.Types.String, typeof(string));
		public static Type Boolean = new Type(Syntax.Types.Boolean, typeof(bool));
		public static Type Character = new Type(Syntax.Types.Character, typeof(char));
		public static Type Byte = new Type(Syntax.Types.Byte, typeof(byte));

		public static Type AnyArray = new Type(Syntax.Types.c_Any + Syntax.Types.ArraySuffix, typeof(object[]));
		public static Type IntegerArray = new Type(Syntax.Types.c_Integer + Syntax.Types.ArraySuffix, typeof(int[]));
		public static Type DecimalArray = new Type(Syntax.Types.c_Decimal + Syntax.Types.ArraySuffix, typeof(double[]));
		public static Type StringArray = new Type(Syntax.Types.c_String + Syntax.Types.ArraySuffix, typeof(string[]));
		public static Type BooleanArray = new Type(Syntax.Types.c_Boolean + Syntax.Types.ArraySuffix, typeof(bool[]));
		public static Type CharacterArray = new Type(Syntax.Types.c_Character + Syntax.Types.ArraySuffix, typeof(char[]));
		public static Type ByteArray = new Type(Syntax.Types.c_Byte + Syntax.Types.ArraySuffix, typeof(byte[]));

		public static readonly Type[] Types = typeof(Type)
					.GetFields(BindingFlags.Public | BindingFlags.Static)
					.Where(member => member.FieldType == typeof(Type))
					.Select(member => (Type)member.GetValue(null))
					.ToArray();

		public static Type GetType(System.Type rep, bool _throw = true)
		{
			foreach (Type type in Types)
			{
				if (type.rep == rep)
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