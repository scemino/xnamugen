using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Gravity")]
	internal class Gravity : StateController
	{
		public Gravity(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
		}

		public override void Run(Combat.Character character)
		{
			character.CurrentVelocity += new Vector2(0, character.BasePlayer.Constants.Vert_acceleration);
		}
	}
}