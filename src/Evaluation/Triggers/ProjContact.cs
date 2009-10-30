using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("ProjContact")]
	static class ProjContact
	{
		public static Number Evaluate(Object state, Number r1, Number r2, Number rhs, Operator compare_type)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (r1.NumberType != NumberType.Int || r2.NumberType != NumberType.Int) return new Number();

			Boolean lookingfor = r2.IntValue > 0;

			Combat.ProjectileInfo projinfo = character.OffensiveInfo.ProjectileInfo;

			Boolean found = (projinfo.Type == ProjectileDataType.Hit || projinfo.Type == ProjectileDataType.Guarded) && (r1.IntValue <= 0 || r1.IntValue == projinfo.ProjectileId);
			if (found == true) found = Number.BinaryOperation(compare_type, new Number(projinfo.Time), rhs).BooleanValue;

			return new Number(lookingfor == found);
		}

		public static Node Parse(ParseState parsestate)
		{
			Node node = parsestate.BuildParenNumberNode(true);
			if (node == null)
			{
#warning Hack
				parsestate.BaseNode.Children.Add(Node.ZeroNode);

				node = parsestate.BaseNode;
			}

			if (parsestate.CurrentOperator != Operator.Equals) return null;
			++parsestate.TokenIndex;

			Node arg1 = parsestate.BuildNode(false);
			if (arg1 == null) return null;

			node.Children.Add(arg1);

			if (parsestate.CurrentSymbol != Symbol.Comma)
			{
#warning Hack
				parsestate.BaseNode.Children.Add(Node.ZeroNode);
				parsestate.BaseNode.Arguments.Add(Operator.Equals);

				return parsestate.BaseNode;
			}

			++parsestate.TokenIndex;

			Operator @operator = parsestate.CurrentOperator;
			if (@operator == Operator.Equals || @operator == Operator.NotEquals)
			{
				++parsestate.TokenIndex;

				Node rangenode = parsestate.BuildRangeNode();
				if (rangenode != null)
				{
					Node newnode = new Node(new Token("ProjContact", new Tokenizing.CustomFunctionData("ProjContact", "ProjContact_", typeof(ProjContact_))));
					newnode.Children.Add(arg1);
					newnode.Children.Add(rangenode.Children[1]);
					newnode.Children.Add(rangenode.Children[2]);
					newnode.Arguments.Add(@operator);
					newnode.Arguments.Add(rangenode.Arguments[1]);
					newnode.Arguments.Add(rangenode.Arguments[2]);

					return newnode;
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

			parsestate.BaseNode.Arguments.Add(@operator);
			parsestate.BaseNode.Children.Add(arg);

			return parsestate.BaseNode;
		}
	}

	static class ProjContact_
	{
		public static Number Evaluate(Object state, Number r1, Number r2, Number pre, Number post, Operator compare_type, Symbol pre_check, Symbol post_check)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			if (r1.NumberType != NumberType.Int || r2.NumberType != NumberType.Int) return new Number();

			Boolean lookingfor = r2.IntValue > 0;

			Combat.ProjectileInfo projinfo = character.OffensiveInfo.ProjectileInfo;

			Boolean found = (projinfo.Type == ProjectileDataType.Hit || projinfo.Type == ProjectileDataType.Guarded) && (r1.IntValue <= 0 || r1.IntValue == projinfo.ProjectileId);
			if (found == true) found = Number.Range(new Number(projinfo.Time), pre, post, compare_type, pre_check, post_check).BooleanValue;

			return new Number(lookingfor == found);
		}

		public static Node Parse(ParseState state)
		{
			return null;
		}
	}
}
