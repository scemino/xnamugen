using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("AuthorName")]
	class AuthorName : Function
	{
		public AuthorName(List<IFunction> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Arguments.Count != 2) return new Number();

			Operator @operator = (Operator)Arguments[0];
			String text = (String)Arguments[1];

			String authorname = character.BasePlayer.Profile.Author;
			if (authorname == null) return new Number();

			Number result = new Number(String.Equals(authorname, text, StringComparison.OrdinalIgnoreCase));

			if (@operator == Operator.Equals) return result;
			if (@operator == Operator.NotEquals) return !result;
			return new Number();
		}

		public static Node Parse(ParseState parsestate)
		{
			Operator @operator = parsestate.CurrentOperator;
			if (@operator != Operator.Equals && @operator != Operator.NotEquals) return null;
			++parsestate.TokenIndex;

			String text = parsestate.CurrentText;
			if (text == null) return null;
			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(@operator);
			parsestate.BaseNode.Arguments.Add(text);
			return parsestate.BaseNode;
		}
	}
}
