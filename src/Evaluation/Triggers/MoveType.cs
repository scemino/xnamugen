using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("MoveType")]
	internal static class MoveType
	{
		public static bool Evaluate(Character character, ref bool error, Operator @operator, xnaMugen.MoveType movetype)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			if (movetype == xnaMugen.MoveType.Unchanged || movetype == xnaMugen.MoveType.None)
			{
				error = true;
				return false;
			}

			switch (@operator)
			{
				case Operator.Equals:
					return movetype == character.MoveType;

				case Operator.NotEquals:
					return movetype != character.MoveType;

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

			var movetype = parsestate.ConvertCurrentToken<xnaMugen.MoveType>();
			if (movetype == xnaMugen.MoveType.Unchanged || movetype == xnaMugen.MoveType.None) return null;

			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(@operator);
			parsestate.BaseNode.Arguments.Add(movetype);
			return parsestate.BaseNode;
		}
	}
}
