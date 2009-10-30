using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimElem")]
	static class AnimElem
	{
		public static Number Evaluate(Object state, Number r1, Number rhs, Operator compare_type)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

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

			Number lhs = new Number(timeoffset);

			Number result = Number.BinaryOperation(compare_type, lhs, rhs);
			if (compare_type == Operator.Equals) return new Number(result.BooleanValue && character.UpdatedAnimation);

			return result;
		}

		public static Node Parse(ParseState state)
		{
			Operator initoperator = state.CurrentOperator;
			switch (initoperator)
			{
#warning Compatability. Equals should be the only one that works.
				case Operator.Equals:
				case Operator.NotEquals:
				case Operator.GreaterEquals:
				case Operator.LesserEquals:
					++state.TokenIndex;
					break;

				default:
					return null;
			}

			Node arg1 = state.BuildNode(false);
			if (arg1 == null) return null;

			state.BaseNode.Children.Add(arg1);

			if (state.CurrentSymbol != Symbol.Comma)
			{
#warning Hack
				state.BaseNode.Children.Add(Node.ZeroNode);
				state.BaseNode.Arguments.Add(Operator.Equals);

				return state.BaseNode;
			}

			++state.TokenIndex;

			Operator @operator = state.CurrentOperator;
			if (@operator == Operator.Equals || @operator == Operator.NotEquals)
			{
				++state.TokenIndex;

				Node rangenode = state.BuildRangeNode();
				if (rangenode != null)
				{
					Node newnode = new Node(new Token("AnimElem", new Tokenizing.CustomFunctionData("AnimElem", "AnimElem_", typeof(AnimElem_))));
					newnode.Children.Add(arg1);
					newnode.Children.Add(rangenode.Children[1]);
					newnode.Children.Add(rangenode.Children[2]);
					newnode.Arguments.Add(@operator);
					newnode.Arguments.Add(rangenode.Arguments[1]);
					newnode.Arguments.Add(rangenode.Arguments[2]);

					return newnode;
				}

				--state.TokenIndex;
			}

			switch (@operator)
			{
				case Operator.Equals:
				case Operator.NotEquals:
				case Operator.GreaterEquals:
				case Operator.LesserEquals:
				case Operator.Lesser:
				case Operator.Greater:
					++state.TokenIndex;
					break;

				default:
					return null;
			}

			Node arg = state.BuildNode(false);
			if (arg == null) return null;

			state.BaseNode.Arguments.Add(@operator);
			state.BaseNode.Children.Add(arg);

			return state.BaseNode;
		}
	}

	static class AnimElem_
	{
		public static Number Evaluate(Object state, Number r1, Number pre, Number post, Operator compare_type, Symbol pre_check, Symbol post_check)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

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

			return Number.Range(new Number(timeoffset), pre, post, compare_type, pre_check, post_check);
		}

		public static Node Parse(ParseState state)
		{
			return null;
		}
	}
}
