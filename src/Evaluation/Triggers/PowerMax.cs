using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("PowerMax")]
	static class PowerMax
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.BasePlayer.Constants.MaximumPower);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
