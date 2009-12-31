using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("StateType")]
	static class StateType
	{
		public static Boolean Evaluate(Object state, ref Boolean error, Operator @operator, xnaMugen.StateType statetype)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			if (statetype == xnaMugen.StateType.Unchanged || statetype == xnaMugen.StateType.None)
			{
				error = true;
				return false;
			}

			switch (@operator)
			{
				case Operator.Equals:
					return statetype == character.StateType;

				case Operator.NotEquals:
					return statetype != character.StateType;

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

			xnaMugen.StateType statetype = parsestate.ConvertCurrentToken<xnaMugen.StateType>();
			if (statetype == xnaMugen.StateType.Unchanged || statetype == xnaMugen.StateType.None) return null;

			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(@operator);
			parsestate.BaseNode.Arguments.Add(statetype);
			return parsestate.BaseNode;
		}
	}
}
