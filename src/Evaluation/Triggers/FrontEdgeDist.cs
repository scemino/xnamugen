using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("FrontEdgeDist")]
	static class FrontEdgeDist
	{
		public static Int32 Evaluate(Object state, ref Boolean error)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			Rectangle camerarect = character.Engine.Camera.ScreenBounds;
			Combat.Stage stage = character.Engine.Stage;

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
