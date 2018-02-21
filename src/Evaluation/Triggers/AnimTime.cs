using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimTime")]
	internal static class AnimTime
	{
        public static int Evaluate(Character character, ref bool error)
		{
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

			if (animation.TotalTime == -1)
			{
				return animtime + 1;
			}

			return animtime - animation.TotalTime;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
