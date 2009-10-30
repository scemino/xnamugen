using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Ctrl")]
	static class Ctrl
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.PlayerControl == PlayerControl.InControl);
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
