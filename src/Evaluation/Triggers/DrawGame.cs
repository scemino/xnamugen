namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("DrawGame")]
	internal static class DrawGame
	{
		public static bool Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
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
