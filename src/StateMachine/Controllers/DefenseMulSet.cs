using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("DefenseMulSet", "DefenceMulSet")]
	class DefenseMulSet : StateController
	{
		public DefenseMulSet(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_multiplier = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			Single? multiplier = EvaluationHelper.AsSingle(character, Multiplier, null);

			if (multiplier == null) return;

			character.DefensiveInfo.DefenseMultiplier = multiplier.Value;
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Multiplier == null) return false;

			return true;
		}

		public Evaluation.Expression Multiplier
		{
			get { return m_multiplier; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_multiplier;

		#endregion
	}
}