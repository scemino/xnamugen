using System;
using System.Diagnostics;

namespace xnaMugen.Combat.Logic
{
	class DisplayRoundNumber : Base
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
			Elements.Base element = Engine.RoundInformation.GetRoundElement(Engine.RoundNumber);

			if (element.DataMap.Type == ElementType.None)
			{
				element = Engine.Elements.GetElement("round.default");
			}

			return element;
		}

		public override Boolean IsFinished()
		{
			return CurrentElement == null;
		}
	}
}