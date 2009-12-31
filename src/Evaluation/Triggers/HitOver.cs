using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitOver")]
	static class HitOver 
	{
		public static Boolean Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.DefensiveInfo.HitTime <= 0;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
