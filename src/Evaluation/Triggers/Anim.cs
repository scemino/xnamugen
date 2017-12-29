namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Anim")]
	internal static class Anim
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null || character.AnimationManager.CurrentAnimation == null)
			{
				error = true;
				return 0;
			}

			return character.AnimationManager.CurrentAnimation.Number;
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
