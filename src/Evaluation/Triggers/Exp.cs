using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Exp")]
	static class Exp
	{
		public static Single Evaluate(Object state, ref Boolean error, Single value)
		{
			return (Single)Math.Exp(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
