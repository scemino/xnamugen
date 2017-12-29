namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P2StateNo")]
	internal static class P2StateNo
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			var opponent = character.GetOpponent();
			if (opponent == null)
			{
				error = true;
				return 0;
			}

			var currentstate = opponent.StateManager.CurrentState;
			return currentstate != null ? currentstate.Number : 0;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
