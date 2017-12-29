using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Ceil")]
	internal static class Ceil
	{
		public static int Evaluate(object state, ref bool error, int value)
		{
			return value;
		}

		public static int Evaluate(object state, ref bool error, float value)
		{
			return (int)Math.Ceiling(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
