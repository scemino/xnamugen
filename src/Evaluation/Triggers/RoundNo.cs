namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("RoundNo")]
	internal static class RoundNo
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			return character.Engine.RoundNumber;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
