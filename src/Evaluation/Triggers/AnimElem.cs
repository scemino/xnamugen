using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimElem")]
	static class AnimElem
	{
		public static Boolean Evaluate(Object state, ref Boolean error, Int32 r1, Int32 rhs, Operator compare_type)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			Animations.Animation animation = character.AnimationManager.CurrentAnimation;
			if (animation == null)
			{
				error = true;
				return false;
			}

			Int32 elementindex = r1 - 1;
			if (elementindex < 0 || elementindex >= animation.Elements.Count)
			{
				error = true;
				return false;
			}

			Int32 elementstarttime = animation.GetElementStartTime(elementindex);
			Int32 animationtime = character.AnimationManager.TimeInAnimation;
			while (animation.TotalTime != -1 && animationtime >= animation.TotalTime)
			{
				Int32 looptime = animation.TotalTime - animation.GetElementStartTime(animation.Loopstart);
				animationtime -= looptime;
			}

			Int32 timeoffset = animationtime - elementstarttime;

			if (character.AnimationManager.IsAnimationFinished == true) return false;

			Boolean result = SpecialFunctions.LogicalOperation(compare_type, timeoffset, rhs);
			return (compare_type == Operator.Equals) ? result && character.UpdatedAnimation : result;
		}

		public static Boolean Evaluate(Object state, ref Boolean error, Int32 r1, Int32 pre, Int32 post, Operator compare_type, Symbol pre_check, Symbol post_check)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			Animations.Animation animation = character.AnimationManager.CurrentAnimation;
			if (animation == null)
			{
				error = true;
				return false;
			}

			Int32 elementindex = r1 - 1;
			if (elementindex < 0 || elementindex >= animation.Elements.Count)
			{
				error = true;
				return false;
			}

			Int32 elementstarttime = animation.GetElementStartTime(elementindex);
			Int32 animationtime = character.AnimationManager.TimeInAnimation;
			while (animation.TotalTime != -1 && animationtime >= animation.TotalTime)
			{
				Int32 looptime = animation.TotalTime - animation.GetElementStartTime(animation.Loopstart);
				animationtime -= looptime;
			}

			Int32 timeoffset = animationtime - elementstarttime;

			if (character.AnimationManager.IsAnimationFinished == true) return false;

			return SpecialFunctions.Range(timeoffset, pre, post, compare_type, pre_check, post_check);
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
					state.BaseNode.Children.Add(rangenode.Children[1]);
					state.BaseNode.Children.Add(rangenode.Children[2]);
					state.BaseNode.Arguments.Add(@operator);
					state.BaseNode.Arguments.Add(rangenode.Arguments[1]);
					state.BaseNode.Arguments.Add(rangenode.Arguments[2]);

					return state.BaseNode;
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
}
