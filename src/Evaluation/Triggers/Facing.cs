namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Facing")]
	internal static class Facing
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			switch (character.CurrentFacing)
			{
				case xnaMugen.Facing.Left:
					return -1;

				case xnaMugen.Facing.Right:
					return 1;

				default:
					error = true;
					return 0;
			}
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
