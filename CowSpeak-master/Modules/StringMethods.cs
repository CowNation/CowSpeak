using CowSpeak.Exceptions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CowSpeak.Modules
{
	[ModuleAttribute.AutoImport]
	[Module("String Methods")]
    public static class StringMethods
    {
		static void VerifyParams(int index, int length = 0)
		{
			if (index < 0)
				throw new BaseException("Index cannot be less than zero");

			if (length < 0)
				throw new BaseException("Length cannot be less than zero");
		}

		[Method(Syntax.Types.String + ".Replace")]
		public static string Replace(Variable obj, string oldValue, string newValue)
		{
			var value = obj.Value.ToString().Replace(oldValue, newValue);
			return value;
		}

		[Method(Syntax.Types.String + ".OccurrencesOf")]
		public static int OccurrencesOf(Variable obj, string counter) => Utils.OccurrencesOf(obj.Value.ToString(), counter);

		[Method(Syntax.Types.String + ".GetBytes")]
		public static byte[] GetBytes(Variable obj, string encoding)
		{
			// this could be done using some janky reflection but this is much faster
			Encoding encoder = null;
			switch (encoding)
            {
				case "UTF-7":
				case "UTF7":
					encoder = Encoding.UTF7;
					break;
				case "BigEndianUnicode":
					encoder = Encoding.BigEndianUnicode;
					break;
				case "UTF-16":
                case "UTF16":
				case "Unicode":
					encoder = Encoding.Unicode;
					break;
				case "Default":
					encoder = Encoding.Default;
					break;
				case "ASCII":
					encoder = Encoding.ASCII;
					break;
				case "UTF-8":
				case "UTF8":
					encoder = Encoding.UTF8;
					break;
				case "UTF-32":
				case "UTF32":
					encoder = Encoding.UTF32;
					break;
				default:
					throw new BaseException("Unknown encoder: " + encoding);
			}

			return encoder.GetBytes((string)obj.Value);
		}

		[Method(Syntax.Types.String + ".Sub" + Syntax.Types.c_String)]
		public static string SubString(Variable obj, int index, int length)
		{
			VerifyParams(index, length);

			string str = obj.Value.ToString();

			if (index + length > str.Length)
				throw new BaseException("Index and length must refer to a location within the string");

			return str.Substring(index, length);
		}

		[Method(Syntax.Types.String + "." + Syntax.Types.c_Character + "At")]
		public static char CharacterAt(Variable obj, int index)
		{
			string str = obj.Value.ToString();

			if (index < 0 || index >= str.Length)
				throw new BaseException("Index must refer to a location within the string");

			return str[index];
		}

		[Method(Syntax.Types.String + ".Length")]
		public static int StringLength(Variable obj) => obj.Value.ToString().Length;

		[Method(Syntax.Types.String + ".Remove")]
		public static string Remove(Variable obj, int index, int length)
		{
			string str = obj.Value.ToString();

			VerifyParams(index, length);

			if (index >= str.Length)
				throw new BaseException("Index must refer to a location within the string");

			if (index + length >= str.Length)
				throw new BaseException("Index and length must refer to a location within the string");

			return str.Remove(index, length);
		}

		[Method(Syntax.Types.String + ".Insert")]
		public static string Insert(Variable obj, int index, string value)
		{
			VerifyParams(index);

			string str = obj.Value.ToString();

			if (index >= str.Length)
				throw new BaseException("Index must refer to a location within the string");

			return str.Insert(index, value);
		}

		[Method(Syntax.Types.String + ".IndexOf")]
		public static int IndexOf(Variable obj, string value) => obj.Value.ToString().IndexOf(value);

		[Method(Syntax.Types.String + ".LastIndexOf")]
		public static int LastIndexOf(Variable obj, string value) => obj.Value.ToString().LastIndexOf(value);

		[Method(Syntax.Types.String + ".To" + Syntax.Types.c_Integer)]
		public static int StringToInteger(Variable obj)
		{
			int o;
			string str = obj.Value.ToString();

			if (Utils.IsHexadecimal(str))
				return int.Parse(str.Substring(2), NumberStyles.HexNumber);

			if (int.TryParse(str, out o))
				return o;
			else
				throw new ConversionException("Could not convert '" + str + "' to an " + Syntax.Types.Integer);
		}

		[Method(Syntax.Types.String + ".To" + Syntax.Types.c_Decimal)]
		public static double ToDecimal(Variable obj)
		{
			double o;
			string str = obj.Value.ToString();
			if (double.TryParse(str, out o))
				return o;
			else
				throw new ConversionException("Could not convert " + str + " to a " + Syntax.Types.Decimal);
		}
	}
}
