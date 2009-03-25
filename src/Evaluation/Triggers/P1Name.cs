using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("P1Name")]
	class P1Name : Function
	{
		public P1Name(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Arguments.Count != 2) return new Number();

			String name = GetName(character);
			if (name == null) return new Number();

			Operator @operator = (Operator)Arguments[0];
			String text = (String)Arguments[1];

			switch (@operator)
			{
				case Operator.Equals:
					return new Number(String.Equals(name, text, StringComparison.OrdinalIgnoreCase));

				case Operator.NotEquals:
					return new Number(!String.Equals(name, text, StringComparison.OrdinalIgnoreCase));

				default:
					return new Number();
			}
		}

		String GetName(Combat.Character character)
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
