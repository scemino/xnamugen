using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Acos")]
	static class Acos
	{
		public static Single Evaluate(Object state, ref Boolean error, Single value)
		{
			if (value < -1 || value > 1)
			{
				error = true;
				return 0;
			}

			return (Single)Math.Acos(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
