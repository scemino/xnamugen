using System;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Ln")]
	internal static class Ln
	{
		public static float Evaluate(Character character, ref bool error, float value)
		{
			return (float)Math.Log10(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
