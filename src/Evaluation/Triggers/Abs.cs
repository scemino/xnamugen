using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Abs")]
	class Abs : Function
	{
		public Abs(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 1) return new Number();

			Number number = Children[0].Evaluate(state);

			if (number.NumberType == NumberType.Int) return new Number(number.IntValue > 0 ? number.IntValue : -number.IntValue);
			if (number.NumberType == NumberType.Float) return new Number(number.FloatValue > 0 ? number.FloatValue : -number.FloatValue);

			return new Number();
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
