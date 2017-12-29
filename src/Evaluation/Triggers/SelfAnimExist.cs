namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("SelfAnimExist")]
	internal static class SelfAnimExist
	{
		public static bool Evaluate(object state, ref bool error, int animationnumber)
		{
			var character = state as Combat.Character;
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
