namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("StateNo")]
	internal static class StateNo
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			var currentstate = character.StateManager.CurrentState;
			return currentstate != null ? currentstate.Number : 0;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
