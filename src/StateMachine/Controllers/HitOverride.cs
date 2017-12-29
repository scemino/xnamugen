using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("HitOverride")]
	internal class HitOverride : StateController
	{
		public HitOverride(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_hitattr = textsection.GetAttribute<Combat.HitAttribute>("attr", null);
			m_slot = textsection.GetAttribute<Evaluation.Expression>("slot", null);
			m_statenumber = textsection.GetAttribute<Evaluation.Expression>("stateno", null);
			m_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			m_forceair = textsection.GetAttribute<Evaluation.Expression>("forceair", null);
		}

		public override void Run(Combat.Character character)
		{
			var slotnumber = EvaluationHelper.AsInt32(character, SlotNumber, 0);
			var statenumber = EvaluationHelper.AsInt32(character, StateNumber, int.MinValue);
			var time = EvaluationHelper.AsInt32(character, Time, 1);
			var forceair = EvaluationHelper.AsBoolean(character, ForceAir, false);

			if (slotnumber < 0 || slotnumber > 7) return;

			character.DefensiveInfo.HitOverrides[slotnumber].Set(Override, statenumber, time, forceair);
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Override == null) return false;

			return true;
		}

		public Combat.HitAttribute Override => m_hitattr;

		public Evaluation.Expression SlotNumber => m_slot;

		public Evaluation.Expression StateNumber => m_statenumber;

		public Evaluation.Expression Time => m_time;

		public Evaluation.Expression ForceAir => m_forceair;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Combat.HitAttribute m_hitattr;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_slot;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_forceair;

		#endregion
	}
}