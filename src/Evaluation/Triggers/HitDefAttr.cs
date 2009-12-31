using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitDefAttr")]
	static class HitDefAttr
	{
		public static Boolean Evaluate(Object state, ref Boolean error, Operator @operator, AttackStateType ast, Combat.HitType[] hittypes)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			if (character.MoveType != xnaMugen.MoveType.Attack) return false;

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
					return heightmatch && datamatch;

				case Operator.NotEquals:
					return !heightmatch || !datamatch;

				default:
					error = true;
					return false;
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

			List<Combat.HitType> hittypes = new List<Combat.HitType>();

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

				hittypes.Add(hittype.Value);
				++parsestate.TokenIndex;
			}

			parsestate.BaseNode.Arguments.Add(hittypes.ToArray());
			return parsestate.BaseNode;
		}
	}
}
