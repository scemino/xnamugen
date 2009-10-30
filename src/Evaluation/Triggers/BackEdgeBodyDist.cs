using System;
using Microsoft.Xna.Framework;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("BackEdgeBodyDist")]
	static class BackEdgeBodyDist
	{
		public static Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Rectangle camerarect = character.Engine.Camera.ScreenBounds;
			Combat.Stage stage = character.Engine.Stage;

			if (character.CurrentFacing == xnaMugen.Facing.Right)
			{
				Int32 value = character.GetLeftEdgePosition(true) - camerarect.Left;
				return new Number(value);
			}
			else if (character.CurrentFacing == xnaMugen.Facing.Left)
			{
				Int32 value = camerarect.Right - character.GetRightEdgePosition(true);
				return new Number(value);
			}
			else
			{
				return new Number();
			}
		}

		public static Node Parse(ParseState state)
		{
			return state.BaseNode;
		}
	}
}
