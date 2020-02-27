namespace CowSpeak
{
	internal class Any
	{
		public ByteArray bytes = new ByteArray();
		public Type Type;

		public Any()
		{

		}
		
		public Any(object obj)
		{
			this.Type = Type.GetTypeFromRep(obj.GetType());
			this.Value = obj;
		}

		public Any(Type Type) => this.Type = Type;

		public Any(Type Type, object initialValue)
		{
			this.Type = Type;
			this.Value = initialValue;
		}

		public object Value
		{
			get
			{
				try
				{
					return System.Convert.ChangeType(bytes.Get(), Type.rep);
				}
				catch (System.OverflowException ex)
				{
					throw new Exception(ex.Message); // ex might be thrown for some other reason
				}
			}
			set
			{
				bytes.Set(value);
			}
		}
	}
}