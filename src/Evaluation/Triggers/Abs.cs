using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Abs")]
	static class Abs
	{
		public static Int32 Evaluate(Object state, ref Boolean error, Int32 value)
		{
			return Math.Abs(value);
		}

		public static Single Evaluate(Object state, ref Boolean error, Single value)
		{
			return Math.Abs(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
