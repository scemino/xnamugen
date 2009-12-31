using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Ceil")]
	static class Ceil
	{
		public static Int32 Evaluate(Object state, ref Boolean error, Int32 value)
		{
			return value;
		}

		public static Int32 Evaluate(Object state, ref Boolean error, Single value)
		{
			return (Int32)Math.Ceiling(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
