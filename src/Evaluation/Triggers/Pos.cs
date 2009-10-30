using System;
using Microsoft.Xna.Framework;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Pos")]
	static class Pos
	{
		public static Number Evaluate(Object state, Axis axis)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			switch (axis)
			{
				case Axis.X:
					Rectangle screenrect = character.Engine.Camera.ScreenBounds;
					return new Number(character.CurrentLocation.X - screenrect.Left - (Mugen.ScreenSize.X / 2));

				case Axis.Y:
					return new Number(character.CurrentLocation.Y);

				default:
					return new Number();
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
