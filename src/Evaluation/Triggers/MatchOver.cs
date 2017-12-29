namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("MatchOver")]
	internal static class MatchOver
	{
		public static bool Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.Engine.IsMatchOver();
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
