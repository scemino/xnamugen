using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("HitVelSet")]
	internal class HitVelSet : StateController
	{
		public HitVelSet(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_velx = textsection.GetAttribute<Evaluation.Expression>("x", null);
			m_vely = textsection.GetAttribute<Evaluation.Expression>("y", null);
		}

		public override void Run(Combat.Character character)
		{
			var velx = EvaluationHelper.AsBoolean(character, XVelocity, true);
			var vely = EvaluationHelper.AsBoolean(character, YVelocity, true);

			var vel = character.DefensiveInfo.GetHitVelocity();

			if (character.DefensiveInfo.Attacker.CurrentFacing == character.CurrentFacing)
			{
				vel *= new Vector2(-1, 1);
			}

			if (velx == false) vel.X = character.CurrentVelocity.X;
			if (vely == false) vel.Y = character.CurrentVelocity.Y;

			character.CurrentVelocity = vel;
		}

		public Evaluation.Expression XVelocity => m_velx;

		public Evaluation.Expression YVelocity => m_vely;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_velx;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_vely;

		#endregion
	}
}