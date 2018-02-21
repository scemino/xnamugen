using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P2Life")]
	internal static class P2Life
	{
		public static int Evaluate(Character character, ref bool error)
		{
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

			return opponent.Life;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
