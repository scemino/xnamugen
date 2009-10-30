using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("DrawGame")]
	static class DrawGame
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.Engine.Team1.VictoryStatus.Lose && character.Engine.Team2.VictoryStatus.Lose);
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
