namespace CowSpeak
{
	public class Variable : Any
	{
		public string Name;

		public Variable(Type vType, string Name) : base(vType)
		{
			this.Name = Name;
		}

		public Variable(Type varType, string Name, object Value) : base(varType)
		{
			this.Name = Name;
			this.Value = Value;
		}
	};
}