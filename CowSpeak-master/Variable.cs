using System;

namespace CowSpeak{
	public class VarType {
		public string Name; // how the type is referenced
		public Type rep; // C# represtation of type
		public Function[] methods = new Function[0]{};

		public VarType(string Name, Type rep, Function[] methods = null){
			this.Name = Name;
			this.rep = rep;

			if (methods != null)
				this.methods = methods;
			else
				this.methods = new Function[0]{};
		}

		public static VarType Void = new VarType("void", typeof(void));
		public static VarType Integer = new VarType("integer", typeof(int));
		public static VarType Decimal = new VarType("decimal", typeof(double));
		public static VarType String = new VarType("string", typeof(string));
		public static VarType Boolean = new VarType("boolean", typeof(bool));
		public static VarType Character = new VarType("character", typeof(char));

		public static VarType[] GetTypes(){
			return new VarType[]{Integer, Decimal, String, Character, Boolean};
		} // returns array of all static types

		public static VarType GetType(string typeName){
			foreach (VarType type in GetTypes()){
				if (type.Name == typeName)
					return type;
			}	

			CowSpeak.FATAL_ERROR("Type '" + typeName + "' does not exist");
			return null;
		}
	}

	public class Any {
		public ByteArray byteArr = new ByteArray();
		public VarType vType;

		public Any(){}
		
		public Any(VarType vType) {
			this.vType = vType;
		}

		public Any(VarType vType, object initialValue) {
			this.vType = vType;
			Set(initialValue);
		}

		public void Set(object to){
			byteArr.Set(to);
		}

		public object Get(){
			try {
				return Convert.ChangeType(byteArr.Get(), vType.rep);
			}
			catch {
				CowSpeak.FATAL_ERROR("Could not read variable '" + (this as Variable).Name + "', it may be corrupted");
				return null;
			} // lazy coder's way out
		}
	}

	public class Variable : Any {
		public string Name;

		public Variable(VarType vType, string Name) : base(vType) {
			this.Name = Name;
		}
	};
}