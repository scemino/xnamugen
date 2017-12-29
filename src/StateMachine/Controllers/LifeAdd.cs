using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("LifeAdd")]
	internal class LifeAdd : StateController
	{
		public LifeAdd(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_life = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_cankill = textsection.GetAttribute<Evaluation.Expression>("kill", null);
			m_abs = textsection.GetAttribute<Evaluation.Expression>("absolute", null);
		}

		public override void Run(Combat.Character character)
		{
			var amount = EvaluationHelper.AsInt32(character, Amount, null);
			var cankill = EvaluationHelper.AsBoolean(character, CanKill, true);
			var absolute = EvaluationHelper.AsBoolean(character, Absolute, false);
			
			if (amount == null) return;

			var scaledamount = amount.Value;
			if (absolute == false) scaledamount = (int)(scaledamount / character.DefensiveInfo.DefenseMultiplier);

			character.Life += scaledamount;

			if (cankill == false && character.Life == 0) character.Life = 1;
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Amount == null) return false;

			return true;
		}

		public Evaluation.Expression Amount => m_life;

		public Evaluation.Expression CanKill => m_cankill;

		public Evaluation.Expression Absolute => m_abs;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_life;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_cankill;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_abs;

		#endregion
	}
}