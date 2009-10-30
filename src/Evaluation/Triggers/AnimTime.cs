using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimTime")]
	static class AnimTime
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Animations.Animation animation = character.AnimationManager.CurrentAnimation;
			Int32 animtime = character.AnimationManager.TimeInAnimation;

			if (animation.TotalTime == -1)
			{
				return new Number(animtime + 1);
			}
			else
			{
				return new Number(animtime - animation.TotalTime);
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
