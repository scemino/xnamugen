using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Exp")]
	internal static class Exp
	{
		public static float Evaluate(object state, ref bool error, float value)
		{
			return (float)Math.Exp(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
