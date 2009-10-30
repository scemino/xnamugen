using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Ln")]
	static class Ln
	{
		public static Number Evaluate(Object state, Number value)
		{
			switch (value.NumberType)
			{
				case NumberType.Int:
				case NumberType.Float:
					return new Number(Math.Log10(value.FloatValue));

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
