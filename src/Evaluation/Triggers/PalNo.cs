namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("PalNo")]
	internal static class PalNo
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.BasePlayer.PaletteNumber + 1;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
