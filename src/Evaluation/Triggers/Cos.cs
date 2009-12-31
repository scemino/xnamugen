using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Cos")]
	static class Cos
	{
		public static Single Evaluate(Object state, ref Boolean error, Single value)
		{
			return (Single)Math.Cos(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
