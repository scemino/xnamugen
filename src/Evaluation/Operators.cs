using System;

namespace xnaMugen.Evaluation
{
	enum Operator
	{
		None,

		[BinaryOperatorMapping("||", typeof(Operations.LogicOr), 0)]
		LogicOr,

		[BinaryOperatorMapping("^^", typeof(Operations.LogicXor), 1)]
		LogicXor,

		[BinaryOperatorMapping("&&", typeof(Operations.LogicAnd), 2)]
		LogicAnd,

		[BinaryOperatorMapping("|", typeof(Operations.BitOr), 3)]
		BitOr,

		[BinaryOperatorMapping("^", typeof(Operations.BitXor), 4)]
		BitXor,

		[BinaryOperatorMapping("&", typeof(Operations.BitAnd), 5)]
		BitAnd,

		[BinaryOperatorMapping("=", typeof(Operations.Equality), 6)]
		Equals,

		[BinaryOperatorMapping("!=", typeof(Operations.Inequality), 6)]
		NotEquals,

		[BinaryOperatorMapping("<", typeof(Operations.LesserThan), 7)]
		Lesser,

		[BinaryOperatorMapping("<=", typeof(Operations.LesserEquals), 7)]
		LesserEquals,

		[BinaryOperatorMapping(">", typeof(Operations.GreaterThan), 7)]
		Greater,

		[BinaryOperatorMapping(">=", typeof(Operations.GreaterEquals), 7)]
		GreaterEquals,

		[BinaryOperatorMapping("+", typeof(Operations.Addition), 8)]
		Plus,

		[BinaryOperatorMapping("-", typeof(Operations.Substraction), 8)]
		Minus,

		[BinaryOperatorMapping("/", typeof(Operations.Division), 9)]
		Divide,

		[BinaryOperatorMapping("*", typeof(Operations.Multiplication), 9)]
		Multiply,

		[BinaryOperatorMapping("%", typeof(Operations.Modulus), 9)]
		Modulus,

		[BinaryOperatorMapping("**", typeof(Operations.Exponent), 10)]
		Exponent,

		[BinaryOperatorMapping(":=", typeof(Operations.Assignment), 11)]
		Assignment,

		[FunctionMapping("!", typeof(Operations.LogicNot))]
		LogicNot,

		[FunctionMapping("~", typeof(Operations.BitNot))]
		BitNot
	}
}