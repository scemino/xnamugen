using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("GameTime")]
	internal static class GameTime
	{
		public static int Evaluate(Character character, ref bool error)
		{
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
