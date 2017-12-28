using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("HitFallDamage")]
	class HitFallDamage : StateController
	{
		public HitFallDamage(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			character.Life -= character.DefensiveInfo.HitDef.FallDamage;
		}
	}
}