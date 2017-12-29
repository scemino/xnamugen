using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("PowerAdd")]
	internal class PowerAdd : StateController
	{
		public PowerAdd(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_power = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			var power = EvaluationHelper.AsInt32(character, Power, null);

			if (power != null) character.BasePlayer.Power += power.Value;
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Power == null) return false;

			return true;
		}

		public Evaluation.Expression Power => m_power;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_power;

		#endregion
	}
}