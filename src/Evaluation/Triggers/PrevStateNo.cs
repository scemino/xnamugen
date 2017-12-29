namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("PrevStateNo")]
	internal static class PrevStateNo
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			var prevstate = character.StateManager.PreviousState;
			return prevstate != null ? prevstate.Number : 0;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
