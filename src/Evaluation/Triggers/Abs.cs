using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Abs")]
	static class Abs
	{
		public static Number Evaluate(Object state, Number value)
		{
			switch (value.NumberType)
			{
				case NumberType.Int:
					return new Number(value.IntValue > 0 ? value.IntValue : -value.IntValue);

				case NumberType.Float:
					return new Number(value.FloatValue > 0 ? value.FloatValue : -value.FloatValue);

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
