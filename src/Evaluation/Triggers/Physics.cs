using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Physics")]
	internal static class Physics 
	{
		public static bool Evaluate(Character character, ref bool error, Operator @operator, xnaMugen.Physics physics)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			if (physics == xnaMugen.Physics.Unchanged || physics == xnaMugen.Physics.None)
			{
				error = true;
				return false;
			}

			switch (@operator)
			{
				case Operator.Equals:
					return physics == character.Physics;

				case Operator.NotEquals:
					return physics != character.Physics;

				default:
					error = true;
					return false;
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			var @operator = parsestate.CurrentOperator;
			if (@operator != Operator.Equals && @operator != Operator.NotEquals) return null;

			++parsestate.TokenIndex;

			var physics = parsestate.ConvertCurrentToken<xnaMugen.Physics>();
			if (physics == xnaMugen.Physics.Unchanged || physics == xnaMugen.Physics.None) return null;

			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(@operator);
			parsestate.BaseNode.Arguments.Add(physics);
			return parsestate.BaseNode;
		}
	}
}
