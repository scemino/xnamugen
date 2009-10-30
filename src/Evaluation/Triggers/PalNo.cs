using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("PalNo")]
	static class PalNo
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.BasePlayer.PaletteNumber + 1);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
