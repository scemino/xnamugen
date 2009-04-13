using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimExist")]
	class AnimExist : Function
	{
		public AnimExist(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Children.Count != 1) return new Number();

			if (character.AnimationManager.IsForeignAnimation == true) return new Number();

			Number r1 = Children[0].Evaluate(state);
			if (r1.NumberType == NumberType.None) return new Number();

			return new Number(character.AnimationManager.HasAnimation(r1.IntValue));
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
