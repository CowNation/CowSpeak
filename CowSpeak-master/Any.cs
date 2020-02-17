namespace CowSpeak
{
	public class Any
	{
		public ByteArray byteArr = new ByteArray();
		public Type vType;

		public Any()
		{

		}
		
		public Any(Type vType) => this.vType = vType;

		public Any(Type vType, object initialValue)
		{
			this.vType = vType;
			Set(initialValue);
		}

		public void Set(object to) => byteArr.Set(to);

		public object Get()
		{
			try
			{
				return System.Convert.ChangeType(byteArr.Get(), vType.rep);
			}
			catch (System.OverflowException)
			{
				throw new Exception("Value was either too large or too small for an " + vType.Name); // ex might be thrown for some other reason, but im too drunk to look into it rn
			}
		}
	}
}