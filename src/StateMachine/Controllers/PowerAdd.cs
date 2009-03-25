using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("PowerAdd")]
	class PowerAdd : StateController
	{
		public PowerAdd(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_power = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32? power = EvaluationHelper.AsInt32(character, Power, null);

			if (power != null) character.BasePlayer.Power += power.Value;
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Power == null) return false;

			return true;
		}

		public Evaluation.Expression Power
		{
			get { return m_power; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_power;

		#endregion
	}
}