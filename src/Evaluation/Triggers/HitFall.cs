using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitFall")]
	static class HitFall
	{
		public static Boolean Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.DefensiveInfo.IsFalling;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
