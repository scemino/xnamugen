using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("GameTime")]
	static class GameTime
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.Engine.TickCount);
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
