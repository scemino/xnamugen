using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("FrontEdgeDist")]
	class FrontEdgeDist : Function
	{
		public FrontEdgeDist(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Rectangle camerarect = character.Engine.Camera.ScreenBounds;
			Combat.Stage stage = character.Engine.Stage;

			if (character.CurrentFacing == xnaMugen.Facing.Right)
			{
				Int32 value = camerarect.Right - character.GetRightEdgePosition(false);
				return new Number(value);
			}
			else if (character.CurrentFacing == xnaMugen.Facing.Left)
			{
				Int32 value = character.GetLeftEdgePosition(false) -camerarect.Left;
				return new Number(value);
			}
			else
			{
				return new Number();
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
