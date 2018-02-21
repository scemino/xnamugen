using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitShakeOver")]
	internal static class HitShakeOver
	{
		public static bool Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.DefensiveInfo.HitShakeTime <= 0;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
