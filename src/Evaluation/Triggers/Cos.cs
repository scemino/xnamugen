using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Cos")]
	class Cos : Function
	{
		public Cos(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 1) return new Number();

			Number number = Children[0].Evaluate(state);

			if (number.NumberType == NumberType.Int) return new Number((Single)Math.Cos(number.IntValue));
			if (number.NumberType == NumberType.Float) return new Number((Single)Math.Cos(number.FloatValue));

			return new Number();
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
