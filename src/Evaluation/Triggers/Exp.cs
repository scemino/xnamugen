using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Exp")]
	static class Exp
	{
		public static Number Evaluate(Object state, Number value)
		{
			switch (value.NumberType)
			{
				case NumberType.Int:
				case NumberType.Float:
					return new Number(Math.Exp(value.FloatValue));

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
