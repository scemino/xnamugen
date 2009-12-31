using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Sin")]
	static class Sin
	{
		public static Single Evaluate(Object state, ref Boolean error, Single value)
		{
			return (Single)Math.Sin(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
