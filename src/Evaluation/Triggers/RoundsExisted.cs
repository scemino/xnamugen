namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("RoundsExisted")]
	internal static class RoundsExisted
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.RoundsExisted;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
