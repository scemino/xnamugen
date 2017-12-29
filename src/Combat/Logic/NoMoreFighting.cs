namespace xnaMugen.Combat.Logic
{
	internal class NoMoreFighting : Base
	{
		public NoMoreFighting(FightEngine engine)
			: base(engine, RoundState.Over)
		{
		}

		protected override Elements.Base GetElement()
		{
			return null;
		}

		public override bool IsFinished()
		{
			return false;
		}
	}
}
