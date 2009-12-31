using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitPauseTime")]
	static class HitPauseTime
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.OffensiveInfo.HitPauseTime;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
