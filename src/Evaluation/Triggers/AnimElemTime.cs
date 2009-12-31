using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimElemTime")]
	static class AnimElemTime
	{
		public static Int32 Evaluate(Object state, ref Boolean error, Int32 value)
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

			//Document states that if element == null, SFalse should be return. Testing seems to show that 0 is returned instead.

			Int32 element_index = value - 1;
			if (animation.Elements.Count <= element_index) return 0;

			Int32 animation_time = character.AnimationManager.TimeInAnimation;
			Int32 element_starttime = animation.GetElementStartTime(element_index);

			Int32 result = animation_time - element_starttime;
			//if (animation.TotalTime != -1 && result >= animation.TotalTime) result = (result % animation.TotalTime);

			return result;
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
