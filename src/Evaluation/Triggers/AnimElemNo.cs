using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimElemNo")]
	static class AnimElemNo
	{
		public static Int32 Evaluate(Object state, ref Boolean error, Int32 timeoffset)
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

			Int32 checktime = animtime + timeoffset;
			if (checktime < 0)
			{
				error = true;
				return 0;
			}

			Int32 elem_index = animation.GetElementFromTime(checktime).Id;
			return elem_index + 1;
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
