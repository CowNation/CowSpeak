namespace CowSpeak
{
	internal class Conversion
	{
		public static bool IsCompatible(Type from, Type to)
		{
			return to == Types.Object || 
				from == Types.Object ||
				from == to || 
				(from == Types.Integer && to == Types.Decimal) || 
				(from == Types.Decimal && to == Types.Integer) || 
				(from == Types.Boolean && to == Types.Integer) || 
				(from == Types.Character && to == Types.Integer) || 
				(from == Types.Integer && to == Types.Character) || 
				(from == Types.Integer && to == Types.Integer64) || 
				(from == Types.DecimalArray && to == Types.IntegerArray) || 
				(from == Types.IntegerArray && to == Types.DecimalArray) ||
				(from == Types.Integer64 && IsCompatible(Types.Integer, to)) ||
				(from == Types.Byte && IsCompatible(Types.Integer, to));
		}
	}
}