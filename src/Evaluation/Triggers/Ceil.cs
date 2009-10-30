using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Ceil")]
	static class Ceil
	{
		public static Number Evaluate(Object state, Number value)
		{
			switch (value.NumberType)
			{
				case NumberType.Int:
					return new Number((Int32)Math.Ceiling(value.FloatValue));

				case NumberType.Float:
					return new Number(Math.Ceiling(value.FloatValue));

				default:
					return new Number();
			}
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
