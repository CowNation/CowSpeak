namespace CowSpeak
{
	public enum TokenType
	{
		AddOperator,
		SubtractOperator,
		MultiplyOperator,
		DivideOperator,
		PowerOperator,
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
		FunctionDefinition,

		VariableIdentifier,
		TypeIdentifier,
		DeleteIdentifier,

		Number,
		String,
		Character,

		IfConditional,
		ElseConditional,
		WhileConditional,
		LoopConditional,

		EndBracket,

		ReturnStatement
	}

	public class Token
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