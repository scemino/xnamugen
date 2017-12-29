namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("CanRecover")]
	internal static class CanRecover
	{
		public static bool Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			return character.DefensiveInfo.IsFalling == false ? false : character.DefensiveInfo.HitDef.FallCanRecover;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
