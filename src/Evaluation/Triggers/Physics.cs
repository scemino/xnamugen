using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Physics")]
	static class Physics 
	{
		public static Boolean Evaluate(Object state, ref Boolean error, Operator @operator, xnaMugen.Physics physics)
		{
			Combat.Character character = state as Combat.Character;
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
			Operator @operator = parsestate.CurrentOperator;
			if (@operator != Operator.Equals && @operator != Operator.NotEquals) return null;

			++parsestate.TokenIndex;

			xnaMugen.Physics physics = parsestate.ConvertCurrentToken<xnaMugen.Physics>();
			if (physics == xnaMugen.Physics.Unchanged || physics == xnaMugen.Physics.None) return null;

			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(@operator);
			parsestate.BaseNode.Arguments.Add(physics);
			return parsestate.BaseNode;
		}
	}
}
