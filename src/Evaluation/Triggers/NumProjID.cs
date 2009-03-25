using System;
using System.Collections.Generic;

namespace xnaMugen.Evaluation.Triggers
{
	[CustomFunction("NumProjID")]
	class NumProjID : Function
	{
		public NumProjID(List<CallBack> children, List<Object> arguments)
			: base(children, arguments)
		{
		}

		public override Number Evaluate(Object state)
		{
			Combat.Character character = state as Combat.Character;
			if (character == null || Children.Count != 1) return new Number();

			Number r1 = Children[0](state);
			if (r1.NumberType != NumberType.Int) return new Number();

			Int32? projectile_id = r1.IntValue > 0 ? r1.IntValue : (Int32?)null;

			Int32 count = 0;
			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Projectile projectile = character.FilterEntityAsProjectile(entity, projectile_id);
				if (projectile == null) continue;

				++count;
			}

			return new Number(count);
		}

		public static Node Parse(ParseState parsestate)
		{
			return parsestate.BuildParenNumberNode(true);
		}
	}
}
