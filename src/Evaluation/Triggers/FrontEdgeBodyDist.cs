namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("FrontEdgeBodyDist")]
	internal static class FrontEdgeBodyDist
	{
		public static int Evaluate(object state, ref bool error)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			var camerarect = character.Engine.Camera.ScreenBounds;
			var stage = character.Engine.Stage;

			switch (character.CurrentFacing)
			{
				case xnaMugen.Facing.Left:
					return character.GetLeftEdgePosition(true) - camerarect.Left;

				case xnaMugen.Facing.Right:
					return camerarect.Right - character.GetRightEdgePosition(true);

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
