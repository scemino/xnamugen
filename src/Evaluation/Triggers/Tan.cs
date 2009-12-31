using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Tan")]
	static class Tan
	{
		public static Single Evaluate(Object state, ref Boolean error, Single value)
		{
			return (Single)Math.Tan(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
