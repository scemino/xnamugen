using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("HitFall")]
	class HitFall : Function
	{
		public HitFall(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}
		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null) return new Number();

			return new Number(character.DefensiveInfo.IsFalling);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BaseNode;
		}
	}
}
