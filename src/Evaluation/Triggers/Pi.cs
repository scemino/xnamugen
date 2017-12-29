using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Pi")]
	internal static class Pi
	{
		public static float Evaluate(object state, ref bool error)
		{
			return (float)Math.PI;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
