using CowSpeak.Exceptions;
using System;

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
			Type = Type.GetType(obj.GetType());
			Value = obj;
		}

		public Any(Type Type) => this.Type = Type;

		public Any(Type Type, object Value) : base()
		{
			this.Type = Type;
			this.Value = Value;
		}

		public object GetValue(System.Type customType)
		{
			try
			{
				object value = Get();

				if (!(value is IConvertible) || customType == typeof(object) || value.GetType() == customType)
					return value;

				return Convert.ChangeType(value, customType);
			}
			catch (OverflowException ex)
			{
				throw new BaseException(ex.Message); // usually caused by a number being assigned an out of range value
			}
		}

		public object Value
		{
			get
			{
				return GetValue(Type.rep);
			}
			set
			{
				Set(value);
			}
		}
	}
}