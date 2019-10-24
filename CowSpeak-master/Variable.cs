using System;

namespace CowSpeak{
	public class VarType {
		public string Name; // how the type is referenced
		public Type rep; // C# represtation of type
		public Function[] methods = new Function[0]{};

		public VarType(string Name, Type rep, Function[] methods){
			this.Name = Name;
			this.rep = rep;
			this.methods = methods;
		}
		
		public static VarType Void = new VarType("void", typeof(void), new Function[0]{});
		public static VarType Integer = new VarType("integer", typeof(int), new Function[0]{});
		public static VarType Decimal = new VarType("decimal", typeof(double), new Function[0]{});
		public static VarType String = new VarType("string", typeof(string), new Function[0]{});
		public static VarType Boolean = new VarType("boolean", typeof(bool), new Function[0]{});
		public static VarType Character = new VarType("character", typeof(char), new Function[0]{});

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
			if (vType == null)
				CowSpeak.FATAL_ERROR("Cannot read object, it's type is null");

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