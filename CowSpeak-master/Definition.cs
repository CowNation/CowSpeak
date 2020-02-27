namespace CowSpeak{
	internal enum DefinitionType
	{
		Language,
		User
	}

	internal struct Definition
	{
		public string from;
		public string to;
		public DefinitionType DefinitionType;
	}
}