using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("TargetVelSet")]
	class TargetVelSet : StateController
	{
		public TargetVelSet(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_targetid = textsection.GetAttribute<Evaluation.Expression>("id", null);
			m_x = textsection.GetAttribute<Evaluation.Expression>("x", null);
			m_y = textsection.GetAttribute<Evaluation.Expression>("y", null);
		}

		public override void Run(Combat.Character character)
		{
			Single? x = EvaluationHelper.AsSingle(character, X, null);
			Single? y = EvaluationHelper.AsSingle(character, Y, null);
			Int32 target_id = EvaluationHelper.AsInt32(character, TargetId, Int32.MinValue);

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character target = character.FilterEntityAsTarget(entity, target_id);
				if (target == null) continue;

				Vector2 velocity = target.CurrentVelocity;

				if (x != null) velocity.X = x.Value;
				if (y != null) velocity.Y = y.Value;

				target.CurrentVelocity = velocity;
			}
		}

		public Evaluation.Expression TargetId
		{
			get { return m_targetid; }
		}

		public Evaluation.Expression X
		{
			get { return m_x; }
		}

		public Evaluation.Expression Y
		{
			get { return m_y; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_targetid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_x;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_y;

		#endregion
	}
}