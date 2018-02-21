using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("SelfAnimExist")]
	internal static class SelfAnimExist
	{
		public static bool Evaluate(Character character, ref bool error, int animationnumber)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.AnimationManager.HasAnimation(animationnumber);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
