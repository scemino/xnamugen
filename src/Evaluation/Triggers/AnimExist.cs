namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimExist")]
	internal static class AnimExist
	{
		public static bool Evaluate(object state, ref bool error, int value)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			if (character.AnimationManager.IsForeignAnimation)
			{
				error = true;
				return false;
			}

			return character.AnimationManager.HasAnimation(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
