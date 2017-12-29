using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("HitFallVel")]
	internal class HitFallVel : StateController
	{
		public HitFallVel(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			var velocity = Vector2.Zero;
			velocity.X = character.DefensiveInfo.HitDef.FallVelocityX ?? character.CurrentVelocity.X;
			velocity.Y = character.DefensiveInfo.HitDef.FallVelocityY;

			character.CurrentVelocity = velocity;
		}
	}
}