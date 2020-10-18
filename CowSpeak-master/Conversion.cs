namespace CowSpeak
{
	internal class Conversion
	{
		public static bool IsCompatible(Type from, Type to)
		{
			return to == Type.Object || 
				from == Type.Object ||
				from == to || 
				(from == Type.Integer && to == Type.Decimal) || 
				(from == Type.Decimal && to == Type.Integer) || 
				(from == Type.Boolean && to == Type.Integer) || 
				(from == Type.Character && to == Type.Integer) || 
				(from == Type.Integer && to == Type.Character) || 
				(from == Type.Integer && to == Type.Integer64) || 
				(from == Type.DecimalArray && to == Type.IntegerArray) || 
				(from == Type.IntegerArray && to == Type.DecimalArray) ||
				(from == Type.Integer64 && IsCompatible(Type.Integer, to)) ||
				(from == Type.Byte && IsCompatible(Type.Integer, to));
		}
	}
}