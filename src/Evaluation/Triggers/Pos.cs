using System;
using Microsoft.Xna.Framework;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Pos")]
	static class Pos
	{
		public static Single Evaluate(Object state, ref Boolean error, Axis axis)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			switch (axis)
			{
				case Axis.X:
					Rectangle screenrect = character.Engine.Camera.ScreenBounds;
					return character.CurrentLocation.X - screenrect.Left - (Mugen.ScreenSize.X / 2);

				case Axis.Y:
					return character.CurrentLocation.Y;

				default:
					error = true;
					return 0;
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			Axis axis = parsestate.ConvertCurrentToken<Axis>();
			if (axis == Axis.None) return null;

			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(axis);
			return parsestate.BaseNode;
		}
	}
}
