using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("HitVelSet")]
	class HitVelSet : StateController
	{
		public HitVelSet(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_velx = textsection.GetAttribute<Evaluation.Expression>("x", null);
			m_vely = textsection.GetAttribute<Evaluation.Expression>("y", null);
		}

		public override void Run(Combat.Character character)
		{
			Boolean velx = EvaluationHelper.AsBoolean(character, XVelocity, true);
			Boolean vely = EvaluationHelper.AsBoolean(character, YVelocity, true);

            Vector2 vel = character.DefensiveInfo.HitVelocity;
            if (character.CurrentFacing == Facing.Left) vel.X = -vel.X;

			if (velx == false) vel.X = character.CurrentVelocity.X;
			if (vely == false) vel.Y = character.CurrentVelocity.Y;

			character.CurrentVelocity = vel;
		}

		public Evaluation.Expression XVelocity
		{
			get { return m_velx; }
		}

		public Evaluation.Expression YVelocity
		{
			get { return m_vely; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_velx;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_vely;

		#endregion
	}
}