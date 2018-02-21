using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimElem")]
	internal static class AnimElem
	{
        public static bool Evaluate(Character character, ref bool error, int r1, int rhs, Operator compareType)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			var animation = character.AnimationManager.CurrentAnimation;
			if (animation == null)
			{
				error = true;
				return false;
			}

			var elementindex = r1 - 1;
			if (elementindex < 0 || elementindex >= animation.Elements.Count)
			{
				error = true;
				return false;
			}

			var elementstarttime = animation.GetElementStartTime(elementindex);
			var animationtime = character.AnimationManager.TimeInAnimation;
			while (animation.TotalTime != -1 && animationtime >= animation.TotalTime)
			{
				var looptime = animation.TotalTime - animation.GetElementStartTime(animation.Loopstart);
				animationtime -= looptime;
			}

			var timeoffset = animationtime - elementstarttime;

			if (character.AnimationManager.IsAnimationFinished) return false;

			var result = SpecialFunctions.LogicalOperation(compareType, timeoffset, rhs);
			return compareType == Operator.Equals ? result && character.UpdatedAnimation : result;
		}

        public static bool Evaluate(Character character, ref bool error, int r1, int pre, int post, Operator compareType, Symbol preCheck, Symbol postCheck)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			var animation = character.AnimationManager.CurrentAnimation;
			if (animation == null)
			{
				error = true;
				return false;
			}

			var elementindex = r1 - 1;
			if (elementindex < 0 || elementindex >= animation.Elements.Count)
			{
				error = true;
				return false;
			}

			var elementstarttime = animation.GetElementStartTime(elementindex);
			var animationtime = character.AnimationManager.TimeInAnimation;
			while (animation.TotalTime != -1 && animationtime >= animation.TotalTime)
			{
				var looptime = animation.TotalTime - animation.GetElementStartTime(animation.Loopstart);
				animationtime -= looptime;
			}

			var timeoffset = animationtime - elementstarttime;

			if (character.AnimationManager.IsAnimationFinished) return false;

			return SpecialFunctions.Range(timeoffset, pre, post, compareType, preCheck, postCheck);
		}

		public static Node Parse(ParseState state)
		{
			var initoperator = state.CurrentOperator;
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

			var arg1 = state.BuildNode(false);
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

			var @operator = state.CurrentOperator;
			if (@operator == Operator.Equals || @operator == Operator.NotEquals)
			{
				++state.TokenIndex;

				var rangenode = state.BuildRangeNode();
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

			var arg = state.BuildNode(false);
			if (arg == null) return null;

			state.BaseNode.Arguments.Add(@operator);
			state.BaseNode.Children.Add(arg);

			return state.BaseNode;
		}
	}
}
