using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("TeamSide")]
	internal static class TeamSide
	{
		public static int Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			switch (character.Team.Side)
			{
				case xnaMugen.TeamSide.Left:
					return 1;

				case xnaMugen.TeamSide.Right:
					return 2;

				default:
					error = true;
					return 0;
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
