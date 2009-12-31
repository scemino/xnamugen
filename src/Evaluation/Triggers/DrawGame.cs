using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("DrawGame")]
	static class DrawGame
	{
		public static Boolean Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
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
