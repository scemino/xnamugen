using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("TargetState")]
	class TargetState : StateController
	{
		public TargetState(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_statenumber = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_targetid = textsection.GetAttribute<Evaluation.Expression>("ID", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32? statenumber = EvaluationHelper.AsInt32(character, StateNumber, null);
			Int32 target_id = EvaluationHelper.AsInt32(character, TargetId, Int32.MinValue);

			if (statenumber == null) return;

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character target = character.FilterEntityAsTarget(entity, target_id);
				if (target == null) continue;

				target.StateManager.ForeignManager = character.StateManager;
				target.StateManager.ChangeState(statenumber.Value);
			}
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (StateNumber == null) return false;

			return true;
		}

		public Evaluation.Expression StateNumber
		{
			get { return m_statenumber; }
		}

		public Evaluation.Expression TargetId
		{
			get { return m_targetid; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_targetid;

		#endregion
	}
}