namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Alive")]
	internal static class Alive
	{
		public static bool Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.Life > 0;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}