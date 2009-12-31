using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("GameTime")]
	static class GameTime
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.Engine.TickCount;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
