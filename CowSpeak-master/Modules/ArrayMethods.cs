using CowSpeak.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CowSpeak.Modules
{
	[ModuleAttribute.AutoImport]
    [Module("Array Methods")]
    public static class ArrayMethods
    {
		[Method(Syntax.Types.ArraySuffix + ".Length")]
		public static int ArrayLength(Variable obj) => ((Array)obj.Value).Length;

		[Method(Syntax.Types.ArraySuffix + ".Set")]
		public static void Set(Variable obj, int index, object value)
		{
			var array = (Array)obj.Value;

			if (index < 0 || index >= array.Length)
				throw new BaseException("Index is out of array bounds");

			if (value.GetType() != array.GetType().GetElementType())
				value = Convert.ChangeType(value, array.GetType().GetElementType());

			array.SetValue(value, index);
			obj.Value = array;
		}

		[Method(Syntax.Types.ArraySuffix + ".Get")]
		public static object Get(Variable obj, int index)
		{
			var array = ((Array)obj.Value);

			if (index < 0 || index >= array.Length)
				throw new BaseException("Index is out of array bounds");

			return array.GetValue(index);
		}
	}
}
