using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("TargetFacing")]
	class TargetFacing : StateController
	{
		public TargetFacing(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_facing = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_targetid = textsection.GetAttribute<Evaluation.Expression>("ID", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32 facing = EvaluationHelper.AsInt32(character, Facing, 0);
			Int32? target_id = EvaluationHelper.AsInt32(character, TargetId, null);

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character target = character.FilterEntityAsTarget(entity, target_id);
				if (target == null) continue;

				if (facing > 0)
				{
					target.CurrentFacing = character.CurrentFacing;
				}
				else if (facing < 0)
				{
					target.CurrentFacing = Misc.FlipFacing(character.CurrentFacing);
				}
			}
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Facing == null) return false;

			return true;
		}

		public Evaluation.Expression Facing
		{
			get { return m_facing; }
		}

		public Evaluation.Expression TargetId
		{
			get { return m_targetid; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_targetid;

		#endregion
	}
}