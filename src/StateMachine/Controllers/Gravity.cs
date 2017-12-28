using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Gravity")]
	class Gravity : StateController
	{
		public Gravity(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			character.CurrentVelocity += new Vector2(0, character.BasePlayer.Constants.Vert_acceleration);
		}
	}
}