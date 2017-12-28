using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("HitFallVel")]
	class HitFallVel : StateController
	{
		public HitFallVel(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			Vector2 velocity = Vector2.Zero;
			velocity.X = character.DefensiveInfo.HitDef.FallVelocityX ?? character.CurrentVelocity.X;
			velocity.Y = character.DefensiveInfo.HitDef.FallVelocityY;

			character.CurrentVelocity = velocity;
		}
	}
}