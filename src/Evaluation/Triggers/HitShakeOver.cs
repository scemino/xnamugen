using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitShakeOver")]
	static class HitShakeOver
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.DefensiveInfo.HitShakeTime <= 0);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
