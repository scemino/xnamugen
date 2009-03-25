using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("MoveHitReset")]
	class MoveHitReset : StateController
	{
		public MoveHitReset(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			character.OffensiveInfo.MoveContact = 0;
			character.OffensiveInfo.MoveGuarded = 0;
			character.OffensiveInfo.MoveHit = 0;
			character.OffensiveInfo.MoveReversed = 0;
		}
	}
}