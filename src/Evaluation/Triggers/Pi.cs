using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Pi")]
	static class Pi
	{
		public static Number Evaluate(Object state)
		{
			return new Number(Math.PI);
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
