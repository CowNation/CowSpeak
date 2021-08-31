using CowSpeak.Exceptions;
using System;
using System.Linq;
using System.Reflection;

namespace CowSpeak
{
	public class Any
	{
		public object obj;
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

		public object ConvertValue(System.Type customType)
		{
			try
			{
				if (obj == null)
					return null;

				if (obj.GetType() == customType)
					return obj;

				if (obj.GetType().IsArray && customType.IsArray)
				{
					Array valueArr = (Array)obj; 
					Array arr = (Array)Activator.CreateInstance(customType, new object[1]
					{
						valueArr.Length
					});
					for (int i = 0; i < valueArr.Length; i++)
					{
						arr.SetValue(Convert.ChangeType(valueArr.GetValue(i), customType.GetElementType()), i);
					}
					return arr;
				}

				if (!(obj is IConvertible))
					return obj;

				return Convert.ChangeType(obj, customType);
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
				return ConvertValue(Type.CSharpType);
			}
			set
			{
				obj = value;
			}
		}
	}
}