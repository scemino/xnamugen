using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitCount")]
	static class HitCount
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.OffensiveInfo.HitCount);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
