namespace CowSpeak
{
	internal enum TokenType
	{
		AddOperator,
		SubtractOperator,
		MultiplyOperator,
		DivideOperator,
		ModuloOperator,
		EqualOperator,
		IsEqualOperator,
		IsNotEqualOperator,
		IsGreaterThanOperator,
		IsLessThanOperator,
		IsGreaterThanOrEqualOperator,
		IsLessThanOrEqualOperator,
		AndOperator,

		FunctionCall,
		FunctionChain,

		VariableIdentifier,
		TypeIdentifier,
		DeleteIdentifier,

		Number,
		String,
		Character,
		Parenthesis,

		IfConditional,
		ElseConditional,
		WhileConditional,
		LoopConditional,

		StartBracket,
		EndBracket,

		ReturnStatement
	}

	internal class Token
	{
		public TokenType type;
		public string identifier;
		
		public Token(TokenType tt, string id)
		{
			type = tt;
			identifier = id;
		}
	}
}