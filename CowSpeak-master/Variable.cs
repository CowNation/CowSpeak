using System;

namespace CowSpeak{
	public class VarType {
		public string Name; // how the type is referenced
		public Type rep; // C# represtation of type

		public VarType(string Name, Type rep){
			this.Name = Name;
			this.rep = rep;
		}
		
		public static VarType Int = new VarType("integer", typeof(int));
		public static VarType Decimal = new VarType("decimal", typeof(double));
		public static VarType String = new VarType("string", typeof(string));
		public static VarType Character = new VarType("character", typeof(char));

		public static VarType[] GetTypes(){
			return new VarType[]{Int, Decimal, String, Character};
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