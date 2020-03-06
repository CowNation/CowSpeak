namespace CowSpeak
{
	internal class Type
	{
		public string Name; // how the type is referenced in the code
		public System.Type rep; // C# representation of type

		public Type(string Name, System.Type rep)
		{
			this.Name = Name;
			this.rep = rep;
		}

		public static Type Any = new Type(Syntax.Types.Any, typeof(void));
		public static Type Void = new Type(Syntax.Types.Void, typeof(void));
		public static Type Integer = new Type(Syntax.Types.Integer, typeof(int));
		public static Type Integer64 = new Type(Syntax.Types.Integer64, typeof(long));
		public static Type Decimal = new Type(Syntax.Types.Decimal, typeof(double));
		public static Type String = new Type(Syntax.Types.String, typeof(string));
		public static Type Boolean = new Type(Syntax.Types.Boolean, typeof(bool));
		public static Type Character = new Type(Syntax.Types.Character, typeof(char));

		public static Type ByteArray = new Type("ByteArray", typeof(byte[]));

		public static Type[] GetTypes()
		{
			return new Type[]{ByteArray, Integer, Integer64, Decimal, String, Character, Boolean, Void, Any};
		} // returns array of all static types

		public static Type GetType(System.Type rep, bool _throw = true)
		{
			foreach (Type type in GetTypes())
			{
				if (type.rep == rep)
					return type;
			}
			if (_throw)
				throw new Exception("Cannot determine type from representation: " + rep.Name);
			return null;
		}

		public static Type GetType(string typeName, bool _throw = false)
		{
			foreach (Type type in GetTypes())
				if (type.Name == typeName)
					return type;

			if (_throw)
				throw new Exception("Type '" + typeName + "' does not exist");

			return null;
		}
	}
}