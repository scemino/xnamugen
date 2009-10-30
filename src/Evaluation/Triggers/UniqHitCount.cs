using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("UniqHitCount")]
	static class UniqHitCount
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.OffensiveInfo.UniqueHitCount);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}

