using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("CanRecover")]
	static class CanRecover
	{
		public static Boolean Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return (character.DefensiveInfo.IsFalling == false) ? false : character.DefensiveInfo.HitDef.FallCanRecover;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
