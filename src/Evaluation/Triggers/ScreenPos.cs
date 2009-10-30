using System;
using Microsoft.Xna.Framework;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("ScreenPos")]
	static class ScreenPos
	{
		public static Number Evaluate(Object state, Axis axis)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Vector2 drawlocation = character.GetDrawLocation() - (Vector2)character.Engine.Camera.Location;

			switch (axis)
			{
				case Axis.X:
					return new Number(drawlocation.X);

				case Axis.Y:
					return new Number(drawlocation.Y);

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
