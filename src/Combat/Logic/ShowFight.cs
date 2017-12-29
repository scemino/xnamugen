namespace xnaMugen.Combat.Logic
{
	internal class ShowFight : Base
	{
		public ShowFight(FightEngine engine)
			: base(engine, RoundState.Intro)
		{
		}

		protected override Elements.Base GetElement()
		{
			return Engine.Elements.GetElement("fight");
		}

		public override bool IsFinished()
		{
			return CurrentElement == null;
		}
	}
}