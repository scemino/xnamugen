using System;
using xnaMugen.Combat;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AuthorName")]
	internal static class AuthorName
	{
        public static bool Evaluate(Character character, ref bool error, Operator @operator, string text)
		{
			if (character == null)
			{
				error = true;
				return false;
			}

			var authorname = character.BasePlayer.Profile.Author;
			if (authorname == null)
			{
				error = true;
				return false;
			}

			var result = string.Equals(authorname, text, StringComparison.OrdinalIgnoreCase);

			switch (@operator)
			{
				case Operator.Equals:
					return result;

				case Operator.NotEquals:
					return !result;

				default:
					error = true;
					return false;
			}
		}

		public static Node Parse(ParseState state)
		{
			var @operator = state.CurrentOperator;
			if (@operator != Operator.Equals && @operator != Operator.NotEquals) return null;
			++state.TokenIndex;

			var text = state.CurrentText;
			if (text == null) return null;
			++state.TokenIndex;

			state.BaseNode.Arguments.Add(@operator);
			state.BaseNode.Arguments.Add(text);
			return state.BaseNode;
		}
	}
}
