using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitOver")]
	static class HitOver 
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.DefensiveInfo.HitTime <= 0);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
