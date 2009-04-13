using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("SelfAnimExist")]
	class SelfAnimExist : Function
	{
		public SelfAnimExist(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Children.Count != 1) return new Number();

			Number animationnumber = Children[0].Evaluate(state);
			if (animationnumber.NumberType != NumberType.Int) return new Number();

			return new Number(character.AnimationManager.HasAnimation(animationnumber.IntValue));
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
