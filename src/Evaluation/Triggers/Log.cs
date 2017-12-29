using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Log")]
	internal static class Log
	{
		public static float Evaluate(object state, ref bool error, float lhs, float rhs)
		{
			return (float)Math.Log(lhs, rhs);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
