using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Win")]
	static class Win
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.Team.VictoryStatus.Win);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}

	[CustomFunction("WinKO")]
	static class WinKO
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.Team.VictoryStatus.WinKO);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}

	[CustomFunction("WinTime")]
	static class WinTime
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.Team.VictoryStatus.WinTime);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}

	[CustomFunction("WinPerfect")]
	static class WinPerfect
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.Team.VictoryStatus.WinPerfect);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}

