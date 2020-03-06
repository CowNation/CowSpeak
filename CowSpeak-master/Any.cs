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
			this.Type = Type.GetType(obj.GetType());
			this.Value = obj;
		}

		public Any(Type Type) => this.Type = Type;

		public Any(Type Type, object Value)
		{
			this.Type = Type;
			this.Value = Value;
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
					throw new Exception(ex.Message); // usually caused by an integer being assigned an out of range value
				}
			}
			set
			{
				bytes.Set(value);
			}
		}
	}
}