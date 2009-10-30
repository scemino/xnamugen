using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("IfElse")]
	static class IfElse
	{
		public static Number Evaluate(Object state, Number r1, Number r2, Number r3)
		{
			if (r1.NumberType == NumberType.None || r2.NumberType == NumberType.None || r3.NumberType == NumberType.None) return new Number();

			return r1.BooleanValue ? r2 : r3;
		}

		public static Node Parse(ParseState parsestate)
		{
			if (parsestate.CurrentSymbol != Symbol.LeftParen) return null;
			++parsestate.TokenIndex;

			Node c1 = parsestate.BuildNode(true);
			if (c1 == null) return null;
			parsestate.BaseNode.Children.Add(c1);

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			Node c2 = parsestate.BuildNode(true);
			if (c2 == null) return null;
			parsestate.BaseNode.Children.Add(c2);

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			Node c3 = parsestate.BuildNode(true);
			if (c3 == null) return null;
			parsestate.BaseNode.Children.Add(c3);

			if (parsestate.CurrentSymbol != Symbol.RightParen) return null;
			++parsestate.TokenIndex;

			return parsestate.BaseNode;
		}
	}
}
