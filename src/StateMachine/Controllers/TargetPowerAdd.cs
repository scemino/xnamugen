using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("TargetPowerAdd")]
	internal class TargetPowerAdd : StateController
	{
		public TargetPowerAdd(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_power = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_targetid = textsection.GetAttribute<Evaluation.Expression>("ID", null);
		}

		public override void Run(Combat.Character character)
		{
			var amount = EvaluationHelper.AsInt32(character, Amount, null);
			var targetId = EvaluationHelper.AsInt32(character, TargetId, int.MinValue);

			if (amount == null) return;

			foreach (var target in character.GetTargets(targetId))
			{
				target.BasePlayer.Power += amount.Value;
			}
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Amount == null) return false;

			return true;
		}

		public Evaluation.Expression Amount => m_power;

		public Evaluation.Expression TargetId => m_targetid;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_power;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_targetid;

		#endregion
	}
}