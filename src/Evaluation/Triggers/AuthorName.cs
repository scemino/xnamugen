using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AuthorName")]
	static class AuthorName
	{
		public static Boolean Evaluate(Object state, ref Boolean error, Operator @operator, String text)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			String authorname = character.BasePlayer.Profile.Author;
			if (authorname == null)
			{
				error = true;
				return false;
			}

			Boolean result = String.Equals(authorname, text, StringComparison.OrdinalIgnoreCase);

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
			Operator @operator = state.CurrentOperator;
			if (@operator != Operator.Equals && @operator != Operator.NotEquals) return null;
			++state.TokenIndex;

			String text = state.CurrentText;
			if (text == null) return null;
			++state.TokenIndex;

			state.BaseNode.Arguments.Add(@operator);
			state.BaseNode.Arguments.Add(text);
			return state.BaseNode;
		}
	}
}
