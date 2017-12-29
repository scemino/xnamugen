namespace xnaMugen.Evaluation
{
	internal enum Symbol
	{
		None,

		[TokenMapping("(")]
		LeftParen,

		[TokenMapping(")")]
		RightParen,

		[TokenMapping("[")]
		LeftBracket,

		[TokenMapping("]")]
		RightBracket,

		[TokenMapping(",")]
		Comma
	}
}