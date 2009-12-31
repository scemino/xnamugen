using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("IfElse")]
	static class IfElse
	{
		public static Int32 Evaluate(Object state, ref Boolean error, Int32 r1, Int32 r2, Int32 r3)
		{
			return r1 != 0 ? r2 : r3;
		}

		public static Single Evaluate(Object state, ref Boolean error, Single r1, Single r2, Single r3)
		{
			return r1 != 0 ? r2 : r3;
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
