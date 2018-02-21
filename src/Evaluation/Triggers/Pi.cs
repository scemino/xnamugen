using System;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Pi")]
	internal static class Pi
	{
		public static float Evaluate(Character character, ref bool error)
		{
			return (float)Math.PI;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
