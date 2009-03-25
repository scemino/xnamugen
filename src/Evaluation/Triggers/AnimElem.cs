using System;
using System.Collections.Generic;
using xnaMugen.Evaluation.Tokenizing;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimElem")]
	class AnimElem : Function
	{
		public AnimElem(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			Number r1 = Children[0](state);
			if (r1.NumberType == NumberType.None) return new Number();

			Animations.Animation animation = character.AnimationManager.CurrentAnimation;

			Int32 elementindex = r1.IntValue - 1;
			if (elementindex < 0 || elementindex >= animation.Elements.Count) return new Number();

			Int32 elementstarttime = animation.GetElementStartTime(elementindex);
			if (elementstarttime == Int32.MinValue) return new Number();

			Int32 animationtime = character.AnimationManager.TimeInAnimation;
			while (animation.TotalTime != -1 && animationtime >= animation.TotalTime)
			{
				Int32 looptime = animation.TotalTime - animation.GetElementStartTime(animation.Loopstart);
				animationtime -= looptime;
			}

			Int32 timeoffset = animationtime - elementstarttime;

			if (character.AnimationManager.IsAnimationFinished == true) return new Number(false);

			return Comparsion(character, new Number(timeoffset));
		}

		public Number Comparsion(Combat.Character character, Number lhs)
		{
			if (character == null) throw new ArgumentNullException("character");

			if (lhs.NumberType == NumberType.None) return new Number();

			Operator compare_type = (Operator)Arguments[0];

			if ((compare_type == Operator.Equals || compare_type == Operator.NotEquals) && Arguments.Count == 3)
			{
				Number pre = Children[1](character);
				Number post = Children[2](character);

				Symbol pre_check = (Symbol)Arguments[1];
				Symbol post_check = (Symbol)Arguments[2];

				return Number.Range(lhs, pre, post, compare_type, pre_check, post_check);
			}
			else
			{
				Number rhs = Children[1](character);

				Number result = Number.BinaryOperation(compare_type, lhs, rhs);
				if (compare_type == Operator.Equals) return new Number(result.BooleanValue && character.UpdatedAnimation);

				return result;
			}
		}

		public static Node Parse(ParseState parsestate)
		{
			Operator initoperator = parsestate.CurrentOperator;
			switch (initoperator)
			{
#warning Compatability. Equals should be the only one that works.
				case Operator.Equals:
				case Operator.NotEquals:
				case Operator.GreaterEquals:
				case Operator.LesserEquals:
					++parsestate.TokenIndex;
					break;

				default:
					return null;
			}

			Node arg1 = parsestate.BuildNode(false);
			if (arg1 == null) return null;

			parsestate.BaseNode.Children.Add(arg1);

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
