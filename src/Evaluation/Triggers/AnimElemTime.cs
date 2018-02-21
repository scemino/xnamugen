using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AnimElemTime")]
	internal static class AnimElemTime
	{
        public static int Evaluate(Character character, ref bool error, int value)
		{
			if (character == null)
			{
				error = true;
				return 0;
			}

			var animation = character.AnimationManager.CurrentAnimation;
			if (animation == null)
			{
				error = true;
				return 0;
			}

			//Document states that if element == null, SFalse should be return. Testing seems to show that 0 is returned instead.

			var elementIndex = value - 1;
			if (animation.Elements.Count <= elementIndex) return 0;

			var animationTime = character.AnimationManager.TimeInAnimation;
			var elementStarttime = animation.GetElementStartTime(elementIndex);

			var result = animationTime - elementStarttime;
			//if (animation.TotalTime != -1 && result >= animation.TotalTime) result = (result % animation.TotalTime);

			return result;
		}

		public static Node Parse(ParseState state)
		{
			return state.BuildParenNumberNode(true);
		}
	}
}
