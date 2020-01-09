namespace CowSpeak
{
	public class Conversion
	{
		public static bool IsCompatible(Type from, Type to)
		{
			return to == Type.Any || from == to || (from == Type.Integer && to == Type.Decimal) || (from == Type.Decimal && to == Type.Integer) || (from == Type.Boolean && to == Type.Integer) || (from == Type.Character && to == Type.Integer) || (from == Type.Integer && to == Type.Character);
		}
	}
}