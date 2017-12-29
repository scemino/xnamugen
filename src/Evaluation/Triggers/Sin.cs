using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Sin")]
	internal static class Sin
	{
		public static float Evaluate(object state, ref bool error, float value)
		{
			return (float)Math.Sin(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
