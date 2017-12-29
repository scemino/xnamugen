namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("Command")]
	internal static class Command
	{
		public static bool Evaluate(object state, ref bool error, Operator @operator, string text)
		{
			var character = state as Combat.Character;
			if (character == null)
			{
				error = true;
				return false;
			}
			var active = character.CommandManager.IsActive(text);

			switch (@operator)
			{
				case Operator.Equals:
					return active;

				case Operator.NotEquals:
					return !active;

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
