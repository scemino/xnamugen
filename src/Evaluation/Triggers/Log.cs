using System;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Log")]
	internal static class Log
	{
		public static float Evaluate(Character character, ref bool error, float lhs, float rhs)
		{
			return (float)Math.Log(lhs, rhs);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
