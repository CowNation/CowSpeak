namespace CowSpeak{
	internal enum DefinitionType
	{
		Static,
		User
	}

	internal struct Definition
	{
		public string from;
		public string to;
		public DefinitionType DefinitionType;
	}
}