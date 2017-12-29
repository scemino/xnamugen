namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimElemNo")]
	internal static class AnimElemNo
	{
		public static int Evaluate(object state, ref bool error, int timeoffset)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			var animation = character.AnimationManager.CurrentAnimation;
			if (animation == null)
			{
				error = true;
				return 0;
			}

			var animtime = character.AnimationManager.TimeInAnimation;

			var checktime = animtime + timeoffset;
			if (checktime < 0)
			{
				error = true;
				return 0;
			}

			var elemIndex = animation.GetElementFromTime(checktime).Id;
			return elemIndex + 1;
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
