using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("ScreenPos")]
	class ScreenPos : Function
	{
		public ScreenPos(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Arguments.Count != 1) return new Number();

			Axis axis = (Axis)Arguments[0];
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
