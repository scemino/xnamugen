using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Null")]
	class Null : StateController
	{
		public Null(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
		}
	}
}
