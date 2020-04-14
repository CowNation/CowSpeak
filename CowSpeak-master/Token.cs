namespace CowSpeak
{
	public enum TokenType
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
		OrOperator,
		
		BitwiseANDOperator,
		BitwiseOROperator,

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

		ReturnStatement
	}

	public class Token
	{
		public TokenType type;
		public string identifier;
		public int Index;
		
		public Token(TokenType type, string identifier, int Index = -1)
		{
			this.type = type;
			this.identifier = identifier;
			this.Index = Index;
		}
	}
}