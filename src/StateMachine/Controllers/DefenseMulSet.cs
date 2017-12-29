using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("DefenseMulSet", "DefenceMulSet")]
	internal class DefenseMulSet : StateController
	{
		public DefenseMulSet(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_multiplier = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			var multiplier = EvaluationHelper.AsSingle(character, Multiplier, null);

			if (multiplier == null) return;

			character.DefensiveInfo.DefenseMultiplier = multiplier.Value;
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