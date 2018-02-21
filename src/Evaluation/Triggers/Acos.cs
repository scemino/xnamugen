using System;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Acos")]
	internal static class Acos
	{
        public static float Evaluate(Character character, ref bool error, float value)
		{
			if (value < -1 || value > 1)
			{
				error = true;
				return 0;
			}

			return (float)Math.Acos(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
