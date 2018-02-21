using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Win")]
	internal static class Win
	{
		public static bool Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.Team.VictoryStatus.Win;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}

	[CustomFunction("WinKO")]
	internal static class WinKO
	{
		public static bool Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.Team.VictoryStatus.WinKO;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}

	[CustomFunction("WinTime")]
	internal static class WinTime
	{
		public static bool Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.Team.VictoryStatus.WinTime;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}

	[CustomFunction("WinPerfect")]
	internal static class WinPerfect
	{
		public static bool Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.Team.VictoryStatus.WinPerfect;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}

