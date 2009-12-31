using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("ProjContact")]
	static class ProjContact
	{
		public static Boolean Evaluate(Object state, ref Boolean error, Int32 proj_id, Int32 r2, Int32 rhs, Operator compare_type)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			Boolean lookingfor = r2 > 0;

			Combat.ProjectileInfo projinfo = character.OffensiveInfo.ProjectileInfo;

			Boolean found = (projinfo.Type == ProjectileDataType.Hit || projinfo.Type == ProjectileDataType.Guarded) && (proj_id <= 0 || proj_id == projinfo.ProjectileId);
			if (found == true) found = SpecialFunctions.LogicalOperation(compare_type, projinfo.Time, rhs);

			return lookingfor == found;
		}

		public static Boolean Evaluate(Object state, ref Boolean error, Int32 proj_id, Int32 r2, Int32 pre, Int32 post, Operator compare_type, Symbol pre_check, Symbol post_check)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			Boolean lookingfor = r2 > 0;

			Combat.ProjectileInfo projinfo = character.OffensiveInfo.ProjectileInfo;

			Boolean found = (projinfo.Type == ProjectileDataType.Hit || projinfo.Type == ProjectileDataType.Guarded) && (proj_id <= 0 || proj_id == projinfo.ProjectileId);
			if (found == true) found = SpecialFunctions.Range(projinfo.Time, pre, post, compare_type, pre_check, post_check);

			return lookingfor == found;
		}

		public static Node Parse(ParseState parsestate)
		{
			Node basenode = parsestate.BuildParenNumberNode(true);
			if (basenode == null)
			{
#warning Hack
				parsestate.BaseNode.Children.Add(Node.ZeroNode);
				basenode = parsestate.BaseNode;
			}

			if (parsestate.CurrentOperator != Operator.Equals) return null;
			++parsestate.TokenIndex;

			Node arg1 = parsestate.BuildNode(false);
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

			Operator @operator = parsestate.CurrentOperator;
			if (@operator == Operator.Equals || @operator == Operator.NotEquals)
			{
				++parsestate.TokenIndex;

				Node rangenode = parsestate.BuildRangeNode();
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

			Node arg = parsestate.BuildNode(false);
			if (arg == null) return null;

			basenode.Arguments.Add(@operator);
			basenode.Children.Add(arg);

			return basenode;
		}
	}
}
