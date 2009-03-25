using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("PlayerIDExist")]
	class PlayerIDExist : Function
	{
		public PlayerIDExist(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Children.Count != 1) return new Number();

			Number id = Children[0](state);
			if (id.NumberType != NumberType.Int) return new Number();

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character c = entity as Combat.Character;
				if (c == null) continue;

				if (c.Id == id.IntValue) return new Number(true);
			}

			return new Number(false);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
