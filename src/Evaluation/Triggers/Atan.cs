using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Atan")]
	static class Atan
	{
		public static Single Evaluate(Object state, ref Boolean error, Single value)
		{
			return (Single)Math.Atan(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
