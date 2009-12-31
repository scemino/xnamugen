using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimExist")]
	static class AnimExist
	{
		public static Boolean Evaluate(Object state, ref Boolean error, Int32 value)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			if (character.AnimationManager.IsForeignAnimation == true)
			{
				error = true;
				return false;
			}

			return character.AnimationManager.HasAnimation(value);
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
