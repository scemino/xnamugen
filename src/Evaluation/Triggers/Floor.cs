using System;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Floor")]
	internal static class Floor
	{
		public static int Evaluate(Character character, ref bool error, int value)
		{
			return value;
		}

		public static int Evaluate(Character character, ref bool error, float value)
		{
			return (int)Math.Floor(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
