namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitFall")]
	internal static class HitFall
	{
		public static bool Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.DefensiveInfo.IsFalling;
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
