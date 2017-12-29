namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("FrontEdgeDist")]
	internal static class FrontEdgeDist
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
					return character.GetLeftEdgePosition(false) - camerarect.Left;

				case xnaMugen.Facing.Right:
					return camerarect.Right - character.GetRightEdgePosition(false);

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
