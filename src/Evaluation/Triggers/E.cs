using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("E")]
	static class E
	{
		public static Single Evaluate(Object state, ref Boolean error)
		{
			return (Single)Math.E;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
