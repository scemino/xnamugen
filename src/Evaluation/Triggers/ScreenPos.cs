using Microsoft.Xna.Framework;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("ScreenPos")]
	internal static class ScreenPos
	{
		public static float Evaluate(Character character, ref bool error, Axis axis)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			var drawlocation = character.GetDrawLocation() - (Vector2)character.Engine.Camera.Location;

			switch (axis)
			{
				case Axis.X:
					return drawlocation.X;

				case Axis.Y:
					return drawlocation.Y;

				default:
					error = true;
					return 0;
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			var axis = parsestate.ConvertCurrentToken<Axis>();
			if (axis == Axis.None) return null;

			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(axis);
			return parsestate.BaseNode;
		}
	}
}
