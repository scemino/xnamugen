namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Ctrl")]
	internal static class Ctrl
	{
		public static bool Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
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
