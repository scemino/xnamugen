namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Life")]
	internal static class Life
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
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
