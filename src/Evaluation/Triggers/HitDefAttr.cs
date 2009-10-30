using System;
using System.Collections.Generic;
using System.Text;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitDefAttr")]
	static class HitDefAttr
	{
		public static Number Evaluate(Object state, Operator @operator, AttackStateType ast, List<Combat.HitType> hittypes)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (character.MoveType != xnaMugen.MoveType.Attack) return new Number(false);

			Combat.HitAttribute attr = character.OffensiveInfo.HitDef.HitAttribute;

			Boolean heightmatch = (attr.AttackHeight & ast) != AttackStateType.None;

			Boolean datamatch = false;
			foreach (Combat.HitType hittype in hittypes)
			{
				if (attr.HasData(hittype) == false) continue;

				datamatch = true;
				break;
			}

			switch (@operator)
			{
				case Operator.Equals:
					return new Number(heightmatch && datamatch);

				case Operator.NotEquals:
					return new Number(!heightmatch || !datamatch);

				default:
					return new Number();
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			Operator @operator = parsestate.CurrentOperator;
			if (@operator != Operator.Equals && @operator != Operator.NotEquals) return null;

			parsestate.BaseNode.Arguments.Add(@operator);
			++parsestate.TokenIndex;

			AttackStateType ast = parsestate.ConvertCurrentToken<AttackStateType>();
			if (ast == AttackStateType.None) return null;

			parsestate.BaseNode.Arguments.Add(ast);
			++parsestate.TokenIndex;

			while (true)
			{
				if (parsestate.CurrentSymbol != Symbol.Comma) break;
				++parsestate.TokenIndex;

				Combat.HitType? hittype = parsestate.ConvertCurrentToken<Combat.HitType?>();
				if (hittype == null)
				{
					--parsestate.TokenIndex;
					break;
				}

				parsestate.BaseNode.Arguments.Add(hittype.Value);
				++parsestate.TokenIndex;
			}

			return parsestate.BaseNode;
		}
	}
}
