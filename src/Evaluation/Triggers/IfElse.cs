namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("IfElse")]
	internal static class IfElse
	{
		public static int Evaluate(object state, ref bool error, int r1, int r2, int r3)
		{
			return r1 != 0 ? r2 : r3;
		}

		public static float Evaluate(object state, ref bool error, float r1, float r2, float r3)
		{
			return r1 != 0 ? r2 : r3;
		}

		public static Node Parse(ParseState parsestate)
		{
			if (parsestate.CurrentSymbol != Symbol.LeftParen) return null;
			++parsestate.TokenIndex;

			var c1 = parsestate.BuildNode(true);
			if (c1 == null) return null;
			parsestate.BaseNode.Children.Add(c1);

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			var c2 = parsestate.BuildNode(true);
			if (c2 == null) return null;
			parsestate.BaseNode.Children.Add(c2);

			if (parsestate.CurrentSymbol != Symbol.Comma) return null;
			++parsestate.TokenIndex;

			var c3 = parsestate.BuildNode(true);
			if (c3 == null) return null;
			parsestate.BaseNode.Children.Add(c3);

			if (parsestate.CurrentSymbol != Symbol.RightParen) return null;
			++parsestate.TokenIndex;

			return parsestate.BaseNode;
		}
	}
}
