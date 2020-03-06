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

		Number,
		String,
		Character,
		Parenthesis,
		Boolean,

		IfConditional,
		ElseConditional,
		WhileConditional,
		LoopConditional,

		StartBracket,
		EndBracket,

		DeleteStatement,
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