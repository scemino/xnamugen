using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("HitOverride")]
	class HitOverride : StateController
	{
		public HitOverride(StateSystem statesystem, String label, TextSection textsection)
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
			Int32 slotnumber = EvaluationHelper.AsInt32(character, SlotNumber, 0);
			Int32 statenumber = EvaluationHelper.AsInt32(character, StateNumber, Int32.MinValue);
			Int32 time = EvaluationHelper.AsInt32(character, Time, 1);
			Boolean forceair = EvaluationHelper.AsBoolean(character, ForceAir, false);

			if (slotnumber < 0 || slotnumber > 7) return;

			character.DefensiveInfo.HitOverrides[slotnumber].Set(Override, statenumber, time, forceair);
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Override == null) return false;

			return true;
		}

		public Combat.HitAttribute Override
		{
			get { return m_hitattr; }
		}

		public Evaluation.Expression SlotNumber
		{
			get { return m_slot; }
		}

		public Evaluation.Expression StateNumber
		{
			get { return m_statenumber; }
		}

		public Evaluation.Expression Time
		{
			get { return m_time; }
		}

		public Evaluation.Expression ForceAir
		{
			get { return m_forceair; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Combat.HitAttribute m_hitattr;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_slot;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_statenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_forceair;

		#endregion
	}
}