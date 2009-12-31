using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Time")]
	static class Time
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			Int32 time = character.StateManager.StateTime;
			if (time < 0) time = 0;

			return time;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}

	[CustomFunction("StateTime")]
	static class StateTime
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			Int32 time = character.StateManager.StateTime;
			if (time < 0) time = 0;

			return time;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
