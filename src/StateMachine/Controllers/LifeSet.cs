using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("LifeSet")]
	internal class LifeSet : StateController
	{
		public LifeSet(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_life = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			var amount = EvaluationHelper.AsInt32(character, Amount, null);

			if (amount == null) return;

			character.Life = amount.Value;
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Amount == null) return false;

			return true;
		}

		public Evaluation.Expression Amount => m_life;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_life;

		#endregion
	}
}