namespace CowSpeak{
	public enum DefinitionType
	{
		Language,
		User
	}

	public struct Definition
	{
		public string from;
		public string to;
		public DefinitionType DefinitionType;
	}
}