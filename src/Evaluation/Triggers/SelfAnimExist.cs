using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("SelfAnimExist")]
	static class SelfAnimExist
	{
		public static Boolean Evaluate(Object state, ref Boolean error, Int32 animationnumber)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.AnimationManager.HasAnimation(animationnumber);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
