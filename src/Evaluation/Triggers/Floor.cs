using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Floor")]
	static class Floor
	{
		public static Int32 Evaluate(Object state, ref Boolean error, Int32 value)
		{
			return value;
		}

		public static Int32 Evaluate(Object state, ref Boolean error, Single value)
		{
			return (Int32)Math.Floor(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
