namespace CowSpeak
{
	public class Any : ByteArray
	{
		public Type Type;

		public Any() : base()
		{

		}
		
		public Any(object obj) : base()
		{
			this.Type = Type.GetType(obj.GetType());
			this.Value = obj;
		}

		public Any(Type Type) => this.Type = Type;

		public Any(Type Type, object Value) : base()
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
					return System.Convert.ChangeType(Get(), Type.rep);
				}
				catch (System.OverflowException ex)
				{
					throw new Exception(ex.Message); // usually caused by an integer being assigned an out of range value
				}
			}
			set
			{
				Set(value);
			}
		}
	}
}