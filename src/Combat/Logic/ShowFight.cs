using System;
using System.Diagnostics;

namespace xnaMugen.Combat.Logic
{
	class ShowFight : Base
	{
		public ShowFight(FightEngine engine)
			: base(engine, RoundState.Intro)
		{
		}

		protected override Elements.Base GetElement()
		{
			return Engine.Elements.GetElement("fight");
		}

		public override Boolean IsFinished()
		{
			return CurrentElement == null;
		}
	}
}