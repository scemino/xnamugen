using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Anim")]
	static class Anim
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || character.AnimationManager.CurrentAnimation == null)
			{
				error = true;
				return 0;
			}

			return character.AnimationManager.CurrentAnimation.Number;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
