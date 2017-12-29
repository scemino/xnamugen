namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Time")]
	internal static class Time
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			var time = character.StateManager.StateTime;
			if (time < 0) time = 0;

			return time;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}

	[CustomFunction("StateTime")]
	internal static class StateTime
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			var time = character.StateManager.StateTime;
			if (time < 0) time = 0;

			return time;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
