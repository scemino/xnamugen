using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Life")]
	internal static class Life
	{
		public static int Evaluate(Character character, ref bool error)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.Life;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
