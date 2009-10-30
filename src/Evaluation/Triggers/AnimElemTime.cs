using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimElemTime")]
	static class AnimElemTime
	{
		public static Number Evaluate(Object state, Number value)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (value.NumberType == NumberType.None) return new Number();

			Animations.Animation animation = character.AnimationManager.CurrentAnimation;
			if (animation == null) return new Number();

			//Document states that if element == null, SFalse should be return. Testing seems to show that 0 is returned instead.

			Int32 element_index = value.IntValue - 1;
			if (animation.Elements.Count <= element_index) return new Number(0);

			Int32 animation_time = character.AnimationManager.TimeInAnimation;
			Int32 element_starttime = animation.GetElementStartTime(element_index);

			Int32 result = animation_time - element_starttime;
			//if (animation.TotalTime != -1 && result >= animation.TotalTime) result = (result % animation.TotalTime);

			return new Number(result);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
