using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Atan")]
	class Atan : Function
	{
		public Atan(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 1) return new Number();

			Number number = Children[0](state);
			if (number.NumberType == NumberType.None) return new Number();

			return new Number(Math.Atan(number.FloatValue));
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
