namespace CowSpeak
{
	public enum TokenType
	{
		None,
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

		// literals
		DecimalLiteral,
		IntegerLiteral,
		Integer64Literal,
		StringLiteral,
		CharacterLiteral,
		BooleanLiteral,

		Parenthesis,

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
		public TokenType type = TokenType.None;
		public string identifier;
		public int Index;

		public Token()
		{

		}

		public Token(TokenType type, string identifier, int index = -1)
		{
			this.type = type;
			this.identifier = identifier;
			this.Index = index;
		}
	}
}