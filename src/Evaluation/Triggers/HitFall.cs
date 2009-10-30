using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitFall")]
	static class HitFall
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.DefensiveInfo.IsFalling);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
