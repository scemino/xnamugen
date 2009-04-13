using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Anim")]
	class Anim : Function
	{
		public Anim(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Animations.Animation animation = character.AnimationManager.CurrentAnimation;
			return animation != null ? new Number(animation.Number) : new Number();
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
