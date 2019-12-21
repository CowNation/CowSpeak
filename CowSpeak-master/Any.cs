namespace CowSpeak {
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
				throw new Exception("Could not read variable '" + (this as Variable).Name + "', it may be corrupted");
			} // lazy coder's way out
		}
	}
}