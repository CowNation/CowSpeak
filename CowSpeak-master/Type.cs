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

		public static Type Any = new Type(Syntax.Types.Any, typeof(void));
		public static Type Void = new Type(Syntax.Types.Void, typeof(void));
		public static Type Integer = new Type(Syntax.Types.Integer, typeof(int));
		public static Type Decimal = new Type(Syntax.Types.Decimal, typeof(double));
		public static Type String = new Type(Syntax.Types.String, typeof(string));
		public static Type Boolean = new Type(Syntax.Types.Boolean, typeof(bool));
		public static Type Character = new Type(Syntax.Types.Character, typeof(char));

		public static Type[] GetTypes()
		{
			return new Type[]{Integer, Decimal, String, Character, Boolean, Void, Any};
		} // returns array of all static types

		public static Type GetType(string typeName)
		{
			foreach (Type type in GetTypes())
				if (type.Name == typeName)
					return type;

			throw new Exception("Type '" + typeName + "' does not exist");
		}
	}
}