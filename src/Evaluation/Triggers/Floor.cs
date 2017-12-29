using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Floor")]
	internal static class Floor
	{
		public static int Evaluate(object state, ref bool error, int value)
		{
			return value;
		}

		public static int Evaluate(object state, ref bool error, float value)
		{
			return (int)Math.Floor(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
