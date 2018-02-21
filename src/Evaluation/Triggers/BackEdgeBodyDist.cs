using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("BackEdgeBodyDist")]
	internal static class BackEdgeBodyDist
	{
        public static int Evaluate(Character character, ref bool error)
		{
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
					return camerarect.Right - character.GetRightEdgePosition(true);

				case xnaMugen.Facing.Right:
					return character.GetLeftEdgePosition(true) - camerarect.Left;

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
