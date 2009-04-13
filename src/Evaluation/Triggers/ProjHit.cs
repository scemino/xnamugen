using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("ProjHit")]
	class ProjHit : Function
	{
		public ProjHit(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Number r1 = Children[0].Evaluate(state);
			Number r2 = Children[1].Evaluate(state);
			if (r1.NumberType != NumberType.Int || r2.NumberType != NumberType.Int) return new Number();

			Boolean lookingfor = r2.IntValue > 0;

			Combat.ProjectileInfo projinfo = character.OffensiveInfo.ProjectileInfo;

			Boolean found = projinfo.Type == ProjectileDataType.Hit && (r1.IntValue <= 0 || r1.IntValue == projinfo.ProjectileId);
			if (found == true)
			{
				found = Comparsion(character, new Number(projinfo.Time)).BooleanValue;
			}

			return new Number(lookingfor == found);
		}

		public Number Comparsion(Combat.Character character, Number lhs)
		{
			if (character == null) throw new ArgumentNullException("character");

			if (lhs.NumberType == NumberType.None) return new Number();

			Operator compare_type = (Operator)Arguments[0];

			if ((compare_type == Operator.Equals || compare_type == Operator.NotEquals) && Arguments.Count == 3)
			{
				Number pre = Children[1].Evaluate(character);
				Number post = Children[2].Evaluate(character);

				Symbol pre_check = (Symbol)Arguments[1];
				Symbol post_check = (Symbol)Arguments[2];

				return Number.Range(lhs, pre, post, compare_type, pre_check, post_check);
			}
			else
			{
				Number rhs = Children[1].Evaluate(character);
				return Number.BinaryOperation(compare_type, lhs, rhs);
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			Node node = parsestate.BuildParenNumberNode(true);
			if (node == null)
			{
#warning Hack
				parsestate.BaseNode.Children.Add(new Node(new Token("0", new Tokenizing.IntData())));

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
				parsestate.BaseNode.Children.Add(new Node(new Token("0", new Tokenizing.IntData())));
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
					rangenode.Children[0] = arg1;
					rangenode.Arguments[0] = @operator;

					return rangenode;
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
}

