using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitDefAttr")]
	internal static class HitDefAttr
	{
		public static bool Evaluate(object state, ref bool error, Operator @operator, AttackStateType ast, Combat.HitType[] hittypes)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			if (character.MoveType != xnaMugen.MoveType.Attack) return false;

			var attr = character.OffensiveInfo.HitDef.HitAttribute;

			var heightmatch = (attr.AttackHeight & ast) != AttackStateType.None;

			var datamatch = false;
			foreach (var hittype in hittypes)
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
			var @operator = parsestate.CurrentOperator;
			if (@operator != Operator.Equals && @operator != Operator.NotEquals) return null;

			parsestate.BaseNode.Arguments.Add(@operator);
			++parsestate.TokenIndex;

			var ast = parsestate.ConvertCurrentToken<AttackStateType>();
			if (ast == AttackStateType.None) return null;

			parsestate.BaseNode.Arguments.Add(ast);
			++parsestate.TokenIndex;

			var hittypes = new List<Combat.HitType>();

			while (true)
			{
				if (parsestate.CurrentSymbol != Symbol.Comma) break;
				++parsestate.TokenIndex;

				var hittype = parsestate.ConvertCurrentToken<Combat.HitType?>();
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
