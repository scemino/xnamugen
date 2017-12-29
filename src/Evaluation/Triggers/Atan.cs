using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Atan")]
	internal static class Atan
	{
		public static float Evaluate(object state, ref bool error, float value)
		{
			return (float)Math.Atan(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
