using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Time")]
	static class Time
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Int32 time = character.StateManager.StateTime;
			if (time < 0) time = 0;

			return new Number(time);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}

	[CustomFunction("StateTime")]
	static class StateTime
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Int32 time = character.StateManager.StateTime;
			if (time < 0) time = 0;

			return new Number(time);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
