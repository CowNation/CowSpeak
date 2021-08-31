using CowSpeak.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CowSpeak.Modules
{
	[ModuleAttribute.AutoImport]
	[Module("Object Methods")]
    public static class ObjectMethods
    {
		[Method("Any.To" + Syntax.Types.c_String)]
		public static string ToString(Variable obj)
		{
			var result = Main._ToString(obj.Value);
			return result;
		}

		[Method("Any.To" + Syntax.Types.c_Byte + Syntax.Types.ArraySuffix)]
		public static unsafe byte[] ToByteArray(Variable obj)
		{
			var handle = GCHandle.Alloc(obj.obj, GCHandleType.Pinned);
			byte[] result = new byte[Marshal.SizeOf(obj.obj)];

			for (int i = 0; i < Marshal.SizeOf(obj.obj); i++)
			{
				result[i] = *(byte*)(handle.AddrOfPinnedObject() + i);
			}

			handle.Free();
			return result;
		}

		[Method("Any.Delete")]
		public static void Delete(Variable obj) => Interpreter.Vars.Remove(obj.Name);

		[Method("Any.InvokeMethod")]
		public static object InvokeMethod(Variable obj, string methodName, object[] parameters)
		{
			var method = obj.Value.GetType().GetMethod(methodName, parameters.Select(x => x.GetType()).ToArray());

			if (method == null)
				throw new BaseException("Cannot find method '" + methodName + "' from object");

			return method.Invoke(obj.Value, parameters);
		}

		[Method("Any.GetIndexer")]
		public static object GetIndexer(Variable obj, object[] index)
		{
			var properties = obj.Value.GetType().GetProperties();
			var property = properties.Where(x => x.GetIndexParameters().Select(y => y.ParameterType).SequenceEqual(index.Select(z => z.GetType()))).FirstOrDefault();

			if (property == null)
				throw new BaseException("Cannot find indexer from object");

			if (!property.CanRead)
				throw new BaseException("Cannot get the value of indexer from object; It's missing a getter");

			return property.GetValue(obj.Value, index);
		}

		[Method("Any.GetProperty")]
		public static object GetProperty(Variable obj, string propertyName)
		{
			var property = obj.Value.GetType().GetProperty(propertyName);

			if (property == null)
				throw new BaseException("Cannot find property '" + propertyName + "' from object");

			if (!property.CanRead)
				throw new BaseException("Cannot get the value of property '" + propertyName + "' from object; It's missing a getter");

			if (property.GetIndexParameters().Length > 0)
				throw new BaseException("Cannot get the value of property '" + propertyName + "' from object; It contains index parameters");

			return property.GetValue(obj.Value, new object[0]);
		}

		[Method("Any.SetProperty")]
		public static void SetProperty(Variable obj, string propertyName, object value)
		{
			var property = obj.Value.GetType().GetProperty(propertyName);

			if (property == null)
				throw new BaseException("Cannot find property '" + propertyName + "' from object");

			if (!property.CanWrite)
				throw new BaseException("Cannot set the value of property '" + propertyName + "' from object; It's missing a setter");

			if (property.GetIndexParameters().Length > 0)
				throw new BaseException("Cannot set the value of property '" + propertyName + "' from object; It contains index parameters");


			property.SetValue(obj.Value, value, new object[0]);
		}

		[Method("Any.GetField")]
		public static object GetField(Variable obj, string fieldName)
		{
			var field = obj.Value.GetType().GetField(fieldName);

			if (field == null)
				throw new BaseException("Cannot find field '" + fieldName + "' from object");

			return field.GetValue(obj.Value);
		}

		[Method("Any.SetField")]
		public static void SetField(Variable obj, string fieldName, object value)
		{
			var field = obj.Value.GetType().GetField(fieldName);

			if (field == null)
				throw new BaseException("Cannot find field '" + fieldName + "' from object");

			if (field.IsLiteral || field.IsInitOnly)
				throw new BaseException("Cannot set field '" + fieldName + "', it isn't writable");

			field.SetValue(obj.Value, value);
		}
	}
}
