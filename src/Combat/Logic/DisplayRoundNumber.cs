namespace xnaMugen.Combat.Logic
{
	internal class DisplayRoundNumber : Base
	{
		public DisplayRoundNumber(FightEngine engine)
			: base(engine, RoundState.Intro)
		{
		}

		protected override void OnFirstTick()
		{
			base.OnFirstTick();

			if (CurrentElement != null)
			{
				DisplayString = Engine.GetSubSystem<StringFormatter>().BuildString(CurrentElement.DataMap.Text, Engine.RoundNumber);
			}
		}

		protected override Elements.Base GetElement()
		{
			var element = Engine.RoundInformation.GetRoundElement(Engine.RoundNumber);

			if (element.DataMap.Type == ElementType.None)
			{
				element = Engine.Elements.GetElement("round.default");
			}

			return element;
		}

		public override bool IsFinished()
		{
			return CurrentElement == null;
		}
	}
}