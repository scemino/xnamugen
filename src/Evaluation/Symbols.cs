using System;

namespace xnaMugen.Evaluation
{
	enum Symbol
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