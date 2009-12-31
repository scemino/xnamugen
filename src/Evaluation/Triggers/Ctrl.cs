using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Ctrl")]
	static class Ctrl
	{
		public static Boolean Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.PlayerControl == PlayerControl.InControl;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
