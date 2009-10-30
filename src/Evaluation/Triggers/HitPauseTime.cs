using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitPauseTime")]
	static class HitPauseTime
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.OffensiveInfo.HitPauseTime);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
