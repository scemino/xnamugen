using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Name")]
	static class Name
	{
		public static Boolean Evaluate(Object state, ref Boolean error, Operator @operator, String text)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			String name = GetName(character);
			if (name == null)
			{
				error = true;
				return false;
			}

			switch (@operator)
			{
				case Operator.Equals:
					return String.Equals(name, text, StringComparison.OrdinalIgnoreCase);

				case Operator.NotEquals:
					return !String.Equals(name, text, StringComparison.OrdinalIgnoreCase);

				default:
					error = true;
					return false;
			}
		}

		static String GetName(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException("character");

			if (character is Combat.Player)
			{
				Combat.Player player = (Combat.Player)character;
				return player.Profile.PlayerName;
			}

			if (character is Combat.Helper)
			{
				Combat.Helper helper = (Combat.Helper)character;
				return helper.Data.Name;
			}

			return null;
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
