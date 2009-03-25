using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimElemTime")]
	class AnimElemTime : Function
	{
		public AnimElemTime(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			if (Children.Count != 1) return new Number();

			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Number r1 = Children[0](state);
			if (r1.NumberType == NumberType.None) return new Number();

			Animations.Animation animation = character.AnimationManager.CurrentAnimation;
			if (animation == null) return new Number();

			//Document states that if element == null, SFalse should be return. Testing seems to show that 0 is returned instead.

			Int32 element_index = r1.IntValue - 1;
			if (animation.Elements.Count <= element_index) return new Number(0);

			Int32 animation_time = character.AnimationManager.TimeInAnimation;
			Int32 element_starttime = animation.GetElementStartTime(element_index);

			Int32 result = animation_time - element_starttime;
			//if (animation.TotalTime != -1 && result >= animation.TotalTime) result = (result % animation.TotalTime);

			return new Number(result);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
