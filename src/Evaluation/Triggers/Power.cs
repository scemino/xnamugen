using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Power")]
	static class Power
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.BasePlayer.Power);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
