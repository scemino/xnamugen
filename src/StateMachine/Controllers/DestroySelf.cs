using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("DestroySelf")]
	class DestroySelf : StateController
	{
		public DestroySelf(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			Combat.Helper helper = character as Combat.Helper;
			if (helper != null) helper.Remove();
		}
	}
}