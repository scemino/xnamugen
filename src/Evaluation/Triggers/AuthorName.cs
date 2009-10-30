using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AuthorName")]
	static class AuthorName
	{
		public static Number Evaluate(Object state, Operator @operator, String text)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			String authorname = character.BasePlayer.Profile.Author;
			if (authorname == null) return new Number();

			Number result = new Number(String.Equals(authorname, text, StringComparison.OrdinalIgnoreCase));

			if (@operator == Operator.Equals) return result;
			if (@operator == Operator.NotEquals) return !result;
			return new Number();
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
