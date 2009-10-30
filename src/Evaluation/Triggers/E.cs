using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("E")]
	static class E
	{
		public static Number Evaluate(Object state)
		{
			return new Number(Math.E);
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
