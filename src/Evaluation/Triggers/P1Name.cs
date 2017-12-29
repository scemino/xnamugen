using System;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P1Name")]
	internal static class P1Name
	{
		public static bool Evaluate(object state, ref bool error, Operator @operator, string text)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}

			var name = GetName(character);
			if (name == null)
			{
				error = true;
				return false;
			}

			switch (@operator)
			{
				case Operator.Equals:
					return string.Equals(name, text, StringComparison.OrdinalIgnoreCase);

				case Operator.NotEquals:
					return !string.Equals(name, text, StringComparison.OrdinalIgnoreCase);

				default:
					error = true;
					return false;
			}
		}

		private static string GetName(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException(nameof(character));

			if (character is Combat.Player)
			{
				var player = (Combat.Player)character;
				return player.Profile.PlayerName;
			}

			if (character is Combat.Helper)
			{
				var helper = (Combat.Helper)character;
				return helper.Data.Name;
			}

			return null;
		}

		public static Node Parse(ParseState parsestate)
		{
			var @operator = parsestate.CurrentOperator;
			if (@operator != Operator.Equals && @operator != Operator.NotEquals) return null;
			++parsestate.TokenIndex;

			var text = parsestate.CurrentText;
			if (text == null) return null;
			++parsestate.TokenIndex;

			parsestate.BaseNode.Arguments.Add(@operator);
			parsestate.BaseNode.Arguments.Add(text);
			return parsestate.BaseNode;
		}
	}
}
