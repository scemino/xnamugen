using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Log")]
	static class Log
	{
		public static Single Evaluate(Object state, ref Boolean error, Single lhs, Single rhs)
		{
			return (Single)Math.Log(lhs, rhs);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
