using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("TargetLifeAdd")]
	class TargetLifeAdd : StateController
	{
		public TargetLifeAdd(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_life = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_targetid = textsection.GetAttribute<Evaluation.Expression>("ID", null);
			m_kill = textsection.GetAttribute<Evaluation.Expression>("kill", null);
			m_abs = textsection.GetAttribute<Evaluation.Expression>("absolute", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32? amount = EvaluationHelper.AsInt32(character, Amount, null);
			Int32 target_id = EvaluationHelper.AsInt32(character, TargetId, Int32.MinValue);
			Boolean cankill = EvaluationHelper.AsBoolean(character, CanKill, true);
			Boolean absolute = EvaluationHelper.AsBoolean(character, Absolute, false);

			if (amount == null) return;

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Character target = character.FilterEntityAsTarget(entity, target_id);
				if (target == null) continue;

				Int32 newamount = amount.Value;

				if (absolute == false && newamount < 0)
				{
					newamount = (Int32)(newamount * character.OffensiveInfo.AttackMultiplier);
					newamount = (Int32)(newamount / target.DefensiveInfo.DefenseMultiplier);
				}

				target.Life += newamount;
				if (target.Life == 0 && cankill == false) target.Life = 1;
			}
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Amount == null) return false;

			return true;
		}

		public Evaluation.Expression Amount
		{
			get { return m_life; }
		}

		public Evaluation.Expression TargetId
		{
			get { return m_targetid; }
		}

		public Evaluation.Expression CanKill
		{
			get { return m_kill; }
		}

		public Evaluation.Expression Absolute
		{
			get { return m_abs; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_life;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_targetid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_kill;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_abs;

		#endregion
	}
}