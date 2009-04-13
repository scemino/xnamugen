using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("IfElse")]
	class IfElse : Function
	{
		public IfElse(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 3) return new Number();


			Number r1 = Children[0].Evaluate(state);
			Number r2 = Children[1].Evaluate(state);
			Number r3 = Children[2].Evaluate(state);

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
