using CowSpeak;
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
	[Module("Conversion Methods")]
    public static class ConversionMethods
    {
		[Method(Syntax.Types.Byte + ".ToInteger")]
		[Method(Syntax.Types.Character + ".ToInteger")]
		public static int ToInteger(Variable obj) => (int)Convert.ChangeType(obj.Value, typeof(int));

		[Method(Syntax.Types.Byte + ".ToHexadecimal")]
		[Method(Syntax.Types.Integer + ".ToHexadecimal")]
		[Method(Syntax.Types.Integer64 + ".ToHexadecimal")]
		public static string ToHexadecimal(Variable obj)
		{
			object value = obj.Value;

			// must get this from reflection because it's type specific (int, long, and byte have their own version)
			var toString = value.GetType().GetMethod("ToString", new System.Type[] { typeof(string) });

			if (toString == null)
				throw new ConversionException("Cannot find ToString(string) format method from type " + obj.Type.CSharpType.Name);

			return (string)toString.Invoke(value, new object[] { "X" });
		}

		[Method(Syntax.Types.Byte + ".ToCharacter")]
		[Method(Syntax.Types.Integer + ".ToCharacter")]
		[Method(Syntax.Types.Integer64 + ".ToCharacter")]
		public static char ToCharacter(Variable obj)
		{
			try
			{
				return (char)Convert.ChangeType(obj.Value, typeof(char));
			}
			catch (InvalidCastException)
			{
				throw new ConversionException("Could not convert " + ((long)obj.Value) + " to a " + Syntax.Types.Character);
			}
		}

		[Function("From" + Syntax.Types.c_Byte + Syntax.Types.ArraySuffix)]
		public static object FromByteArray(string typeName, byte[] bytes)
		{
			var type = Type.GetType(typeName);

			GCHandle handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);

			object result;
			try
			{
				result = Marshal.PtrToStructure(handle.AddrOfPinnedObject(), type.CSharpType);
			}
			catch (MissingMethodException)
			{
				handle.Free();
				throw new BaseException("Cannot convert byte array " + Main._ToString(bytes) + " to a " + typeName);
			}

			handle.Free();
			return result;
		}
	}
}
