using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Lose")]
	internal static class Lose
	{
		public static bool Evaluate(Character character, ref bool error)
		{
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
	internal static class LoseKO
	{
		public static bool Evaluate(Character character, ref bool error)
		{
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
	internal static class LoseTime
	{
		public static bool Evaluate(Character character, ref bool error)
		{
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
