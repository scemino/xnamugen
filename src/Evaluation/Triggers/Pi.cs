using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Pi")]
	static class Pi
	{
		public static Single Evaluate(Object state, ref Boolean error)
		{
			return (Single)Math.PI;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
