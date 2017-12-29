using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("TargetState")]
	internal class TargetState : StateController
	{
		public TargetState(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_statenumber = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_targetid = textsection.GetAttribute<Evaluation.Expression>("ID", null);
		}

		public override void Run(Combat.Character character)
		{
			var statenumber = EvaluationHelper.AsInt32(character, StateNumber, null);
			var targetId = EvaluationHelper.AsInt32(character, TargetId, int.MinValue);

			if (statenumber == null) return;

			foreach (var target in character.GetTargets(targetId))
			{
				target.StateManager.ForeignManager = character.StateManager;
				target.StateManager.ChangeState(statenumber.Value);
			}
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (StateNumber == null) return false;

			return true;
		}

		public Evaluation.Expression StateNumber => m_statenumber;

		public Evaluation.Expression TargetId => m_targetid;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_targetid;

		#endregion
	}
}