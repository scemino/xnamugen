using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Lose")]
	static class Lose
	{
		public static Boolean Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.Team.VictoryStatus.Lose;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}

	[CustomFunction("LoseKO")]
	static class LoseKO
	{
		public static Boolean Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.Team.VictoryStatus.LoseKO;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}

	[CustomFunction("LoseTime")]
	static class LoseTime
	{
		public static Boolean Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.Team.VictoryStatus.LoseTime;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
