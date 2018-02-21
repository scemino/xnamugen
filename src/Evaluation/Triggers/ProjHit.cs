using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("ProjHit")]
	internal static class ProjHit
	{
		public static bool Evaluate(Character character, ref bool error, int projId, int r2, int rhs, Operator compareType)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			var lookingfor = r2 > 0;

			var projinfo = character.OffensiveInfo.ProjectileInfo;

			var found = projinfo.Type == ProjectileDataType.Hit && (projId <= 0 || projId == projinfo.ProjectileId);
			if (found) found = SpecialFunctions.LogicalOperation(compareType, projinfo.Time, rhs);

			return lookingfor == found;
		}

		public static bool Evaluate(Character character, ref bool error, int projId, int r2, int pre, int post, Operator compareType, Symbol preCheck, Symbol postCheck)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			var lookingfor = r2 > 0;

			var projinfo = character.OffensiveInfo.ProjectileInfo;

			var found = projinfo.Type == ProjectileDataType.Hit&& (projId <= 0 || projId == projinfo.ProjectileId);
			if (found) found = SpecialFunctions.Range(projinfo.Time, pre, post, compareType, preCheck, postCheck);

			return lookingfor == found;
		}

		public static Node Parse(ParseState parsestate)
		{
			var basenode = parsestate.BuildParenNumberNode(true);
			if (basenode == null)
			{
#warning Hack
				parsestate.BaseNode.Children.Add(Node.ZeroNode);
				basenode = parsestate.BaseNode;
			}

			if (parsestate.CurrentOperator != Operator.Equals) return null;
			++parsestate.TokenIndex;

			var arg1 = parsestate.BuildNode(false);
			if (arg1 == null) return null;

			basenode.Children.Add(arg1);

			if (parsestate.CurrentSymbol != Symbol.Comma)
			{
#warning Hack
				basenode.Children.Add(Node.ZeroNode);
				basenode.Arguments.Add(Operator.Equals);

				return basenode;
			}

			++parsestate.TokenIndex;

			var @operator = parsestate.CurrentOperator;
			if (@operator == Operator.Equals || @operator == Operator.NotEquals)
			{
				++parsestate.TokenIndex;

				var rangenode = parsestate.BuildRangeNode();
				if (rangenode != null)
				{
					basenode.Children.Add(rangenode.Children[1]);
					basenode.Children.Add(rangenode.Children[2]);
					basenode.Arguments.Add(@operator);
					basenode.Arguments.Add(rangenode.Arguments[1]);
					basenode.Arguments.Add(rangenode.Arguments[2]);

					return basenode;
				}

				--parsestate.TokenIndex;
			}

			switch (@operator)
			{
				case Operator.Equals:
				case Operator.NotEquals:
				case Operator.GreaterEquals:
				case Operator.LesserEquals:
				case Operator.Lesser:
				case Operator.Greater:
					++parsestate.TokenIndex;
					break;

				default:
					return null;
			}

			var arg = parsestate.BuildNode(false);
			if (arg == null) return null;

			basenode.Arguments.Add(@operator);
			basenode.Children.Add(arg);

			return basenode;
		}
	}
}
