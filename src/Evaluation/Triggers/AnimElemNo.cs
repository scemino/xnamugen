using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimElemNo")]
	static class AnimElemNo
	{
		public static Number Evaluate(Object state, Number value)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (value.NumberType == NumberType.None) return new Number();

			Animations.Animation animation = character.AnimationManager.CurrentAnimation;
			Int32 timeoffset = value.IntValue;
			Int32 animtime = character.AnimationManager.TimeInAnimation;

			Int32 checktime = animtime + timeoffset;
			if (checktime < 0) return new Number();

			Int32 elem_index = animation.GetElementFromTime(checktime).Id;
			return new Number(elem_index + 1);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
