using System;

namespace xnaMugen.Evaluation
{
	enum Operator
	{
		None,

		[BinaryOperatorMapping("||", "LogicalOr", 0)]
		LogicalOr,

		[BinaryOperatorMapping("^^", "LogicalXor", 1)]
		LogicalXor,

		[BinaryOperatorMapping("&&", "LogicalAnd", 2)]
		LogicalAnd,

		[BinaryOperatorMapping("|", "BinaryOr", 3)]
		BinaryOr,

		[BinaryOperatorMapping("^", "BinaryXor", 4)]
		BinaryXor,

		[BinaryOperatorMapping("&", "BinaryAnd", 5)]
		BinaryAnd,

		[BinaryOperatorMapping("=", "op_Equality", 6)]
		Equals,

		[BinaryOperatorMapping("!=", "op_Inequality", 6)]
		NotEquals,

		[BinaryOperatorMapping("<", "op_LessThan", 7)]
		Lesser,

		[BinaryOperatorMapping("<=", "op_LessThanOrEqual", 7)]
		LesserEquals,

		[BinaryOperatorMapping(">", "op_GreaterThan", 7)]
		Greater,

		[BinaryOperatorMapping(">=", "op_GreaterThanOrEqual", 7)]
		GreaterEquals,

		[BinaryOperatorMapping("+", "op_Addition", 8)]
		Plus,

		[BinaryOperatorMapping("-", "op_Subtraction", 8)]
		Minus,

		[BinaryOperatorMapping("/", "op_Division", 9)]
		Divide,

		[BinaryOperatorMapping("*", "op_Multiply", 9)]
		Multiply,

		[BinaryOperatorMapping("%", "op_Modulus", 9)]
		Modulus,

		[BinaryOperatorMapping("**", "Power", 10)]
		Exponent,

		[BinaryOperatorMapping(":=", "_Assignment", 11)]
		Assignment,

		[UnaryOperatorMappingAttribute("!", "LogicalNot")]
		LogicalNot,

		[UnaryOperatorMappingAttribute("~", "BinaryNot")]
		BitNot
	}
}