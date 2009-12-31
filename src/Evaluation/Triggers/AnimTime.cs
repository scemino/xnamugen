using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimTime")]
	static class AnimTime
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			Animations.Animation animation = character.AnimationManager.CurrentAnimation;
			if (animation == null)
			{
				error = true;
				return 0;
			}

			Int32 animtime = character.AnimationManager.TimeInAnimation;

			if (animation.TotalTime == -1)
			{
				return animtime + 1;
			}
			else
			{
				return animtime - animation.TotalTime;
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
