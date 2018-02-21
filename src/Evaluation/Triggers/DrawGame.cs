using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("DrawGame")]
	internal static class DrawGame
	{
		public static bool Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.Engine.Team1.VictoryStatus.Lose && character.Engine.Team2.VictoryStatus.Lose;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
