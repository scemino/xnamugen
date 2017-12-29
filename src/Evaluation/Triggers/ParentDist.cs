using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("ParentDist")]
	internal static class ParentDist
	{
		public static float Evaluate(object state, ref bool error, Axis axis)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return 0;
			}

			var helper = character as Combat.Helper;
			if (helper == null)
			{
				error = true;
				return 0;
			}

			switch (axis)
			{
				case Axis.X:
					var distance = Math.Abs(helper.CurrentLocation.X - helper.Parent.CurrentLocation.X);
					if (helper.CurrentFacing == xnaMugen.Facing.Right)
					{
						return helper.Parent.CurrentLocation.X >= helper.CurrentLocation.X ? distance : -distance;
					}
					else
					{
						return helper.Parent.CurrentLocation.X >= helper.CurrentLocation.X ? -distance : distance;
					}

				case Axis.Y:
					return helper.Parent.CurrentLocation.Y - helper.CurrentLocation.Y;

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
