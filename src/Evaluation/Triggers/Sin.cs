using System;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Sin")]
	internal static class Sin
	{
		public static float Evaluate(Character character, ref bool error, float value)
		{
			return (float)Math.Sin(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
