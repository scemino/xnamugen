using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Cos")]
	static class Cos
	{
		public static Number Evaluate(Object state, Number value)
		{
			switch (value.NumberType)
			{
				case NumberType.Int:
				case NumberType.Float:
					return new Number(Math.Cos(value.FloatValue));

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
