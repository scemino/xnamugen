using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AttackMulSet")]
	internal class AttackMulSet : StateController
	{
		public AttackMulSet(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_multiplier = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			var multiplier = EvaluationHelper.AsSingle(character, Multiplier, null);

			if (multiplier == null) return;

			character.OffensiveInfo.AttackMultiplier = multiplier.Value;
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Multiplier == null) return false;

			return true;
		}

		public Evaluation.Expression Multiplier => m_multiplier;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_multiplier;

		#endregion
	}
}