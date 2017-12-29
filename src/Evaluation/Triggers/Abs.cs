using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Abs")]
	internal static class Abs
	{
		public static int Evaluate(object state, ref bool error, int value)
		{
			return Math.Abs(value);
		}

		public static float Evaluate(object state, ref bool error, float value)
		{
			return Math.Abs(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
