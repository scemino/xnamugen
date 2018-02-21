using System;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("E")]
	internal static class E
	{
		public static float Evaluate(Character character, ref bool error)
		{
			return (float)Math.E;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
