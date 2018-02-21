using System;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Tan")]
	internal static class Tan
	{
		public static float Evaluate(Character character, ref bool error, float value)
		{
			return (float)Math.Tan(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
