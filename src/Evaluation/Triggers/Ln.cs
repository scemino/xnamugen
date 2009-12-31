using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Ln")]
	static class Ln
	{
		public static Single Evaluate(Object state, ref Boolean error, Single value)
		{
			return (Single)Math.Log10(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
