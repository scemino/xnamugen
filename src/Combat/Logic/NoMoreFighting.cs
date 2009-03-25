using System;

namespace xnaMugen.Combat.Logic
{
	class NoMoreFighting : Base
	{
		public NoMoreFighting(FightEngine engine)
			: base(engine, RoundState.Over)
		{
		}

		protected override Elements.Base GetElement()
		{
			return null;
		}

		public override Boolean IsFinished()
		{
			return false;
		}
	}
}
