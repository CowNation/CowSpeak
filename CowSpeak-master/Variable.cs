namespace CowSpeak{
	public class Type {
		public string Name; // how the type is referenced in the code
		public System.Type rep; // C# representation of type

		public Type(string Name, System.Type rep){
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

		public static Type[] GetTypes(){
			return new Type[]{Integer, Decimal, String, Character, Boolean, Void, Any};
		} // returns array of all static types

		public static Type GetType(string typeName){
			foreach (Type type in GetTypes()){
				if (type.Name == typeName)
					return type;
			}	

			CowSpeak.FatalError("Type '" + typeName + "' does not exist");
			return null;
		}
	}

	public class Any {
		public ByteArray byteArr = new ByteArray();
		public Type vType;

		public Any(){}
		
		public Any(Type vType) {
			this.vType = vType;
		}

		public Any(Type vType, object initialValue) {
			this.vType = vType;
			Set(initialValue);
		}

		public void Set(object to){
			byteArr.Set(to);
		}

		public object Get(){
			try {
				return System.Convert.ChangeType(byteArr.Get(), vType.rep);
			}
			catch {
				CowSpeak.FatalError("Could not read variable '" + (this as Variable).Name + "', it may be corrupted");
				return null;
			} // lazy coder's way out
		}
	}

	public class Variable : Any {
		public string Name;

		public Variable(Type vType, string Name) : base(vType) {
			this.Name = Name;
		}

		public Variable(Type varType, string Name, object Value) : base(varType) {
			this.Name = Name;
			Set(Value);
		}
	};
}