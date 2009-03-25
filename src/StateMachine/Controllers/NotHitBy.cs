using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("NotHitBy")]
	class NotHitBy : StateController
	{
		public NotHitBy(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_hitattr1 = textsection.GetAttribute<Combat.HitAttribute>("value", null);
			m_hitattr2 = textsection.GetAttribute<Combat.HitAttribute>("value2", null);
			m_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32 time = EvaluationHelper.AsInt32(character, Time, 1);

			if (HitAttribute1 != null)
			{
				character.DefensiveInfo.HitBy1.Set(HitAttribute1, time, true);
			}

			if (HitAttribute2 != null)
			{
				character.DefensiveInfo.HitBy2.Set(HitAttribute2, time, true);
			}
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if ((HitAttribute1 != null) == (HitAttribute2 != null)) return false;

			return true;
		}

		public Combat.HitAttribute HitAttribute1
		{
			get { return m_hitattr1; }
		}

		public Combat.HitAttribute HitAttribute2
		{
			get { return m_hitattr2; }
		}

		public Evaluation.Expression Time
		{
			get { return m_time; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Combat.HitAttribute m_hitattr1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Combat.HitAttribute m_hitattr2;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_time;

		#endregion
	}
}