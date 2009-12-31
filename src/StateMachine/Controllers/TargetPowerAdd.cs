using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("TargetPowerAdd")]
	class TargetPowerAdd : StateController
	{
		public TargetPowerAdd(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_power = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_targetid = textsection.GetAttribute<Evaluation.Expression>("ID", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32? amount = EvaluationHelper.AsInt32(character, Amount, null);
			Int32 target_id = EvaluationHelper.AsInt32(character, TargetId, Int32.MinValue);

			if (amount == null) return;

			foreach (Combat.Character target in character.GetTargets(target_id))
			{
				target.BasePlayer.Power += amount.Value;
			}
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Amount == null) return false;

			return true;
		}

		public Evaluation.Expression Amount
		{
			get { return m_power; }
		}

		public Evaluation.Expression TargetId
		{
			get { return m_targetid; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_power;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_targetid;

		#endregion
	}
}