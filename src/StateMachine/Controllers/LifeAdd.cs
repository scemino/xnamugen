using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("LifeAdd")]
	class LifeAdd : StateController
	{
		public LifeAdd(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_life = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_cankill = textsection.GetAttribute<Evaluation.Expression>("kill", null);
			m_abs = textsection.GetAttribute<Evaluation.Expression>("absolute", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32? amount = EvaluationHelper.AsInt32(character, Amount, null);
			Boolean cankill = EvaluationHelper.AsBoolean(character, CanKill, true);
			Boolean absolute = EvaluationHelper.AsBoolean(character, Absolute, false);
			
			if (amount == null) return;

			Int32 scaledamount = amount.Value;
			if (absolute == false) scaledamount = (Int32)(scaledamount / character.DefensiveInfo.DefenseMultiplier);

			character.Life += scaledamount;

			if (cankill == false && character.Life == 0) character.Life = 1;
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

		public Evaluation.Expression CanKill
		{
			get { return m_cankill; }
		}

		public Evaluation.Expression Absolute
		{
			get { return m_abs; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_life;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_cankill;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_abs;

		#endregion
	}
}