using System;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Cos")]
	internal static class Cos
	{
		public static float Evaluate(Character character, ref bool error, float value)
		{
			return (float)Math.Cos(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
