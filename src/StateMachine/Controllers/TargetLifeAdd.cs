using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("TargetLifeAdd")]
	internal class TargetLifeAdd : StateController
	{
		public TargetLifeAdd(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_life = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_targetid = textsection.GetAttribute<Evaluation.Expression>("ID", null);
			m_kill = textsection.GetAttribute<Evaluation.Expression>("kill", null);
			m_abs = textsection.GetAttribute<Evaluation.Expression>("absolute", null);
		}

		public override void Run(Combat.Character character)
		{
			var amount = EvaluationHelper.AsInt32(character, Amount, null);
			var targetId = EvaluationHelper.AsInt32(character, TargetId, int.MinValue);
			var cankill = EvaluationHelper.AsBoolean(character, CanKill, true);
			var absolute = EvaluationHelper.AsBoolean(character, Absolute, false);

			if (amount == null) return;

			foreach (var target in character.GetTargets(targetId))
			{
				var newamount = amount.Value;

				if (absolute == false && newamount < 0)
				{
					newamount = (int)(newamount * character.OffensiveInfo.AttackMultiplier);
					newamount = (int)(newamount / target.DefensiveInfo.DefenseMultiplier);
				}

				target.Life += newamount;
				if (target.Life == 0 && cankill == false) target.Life = 1;
			}
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Amount == null) return false;

			return true;
		}

		public Evaluation.Expression Amount => m_life;

		public Evaluation.Expression TargetId => m_targetid;

		public Evaluation.Expression CanKill => m_kill;

		public Evaluation.Expression Absolute => m_abs;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_life;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_targetid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_kill;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_abs;

		#endregion
	}
}