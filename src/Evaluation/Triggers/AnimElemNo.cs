using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimElemNo")]
	class AnimElemNo : Function
	{
		public AnimElemNo(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Children.Count != 1) return new Number();

			Number r1 = Children[0].Evaluate(state);
			if (r1.NumberType == NumberType.None) return new Number();

			Animations.Animation animation = character.AnimationManager.CurrentAnimation;
			Int32 timeoffset = r1.IntValue;
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
