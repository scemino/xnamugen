using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("HitAdd")]
	class HitAdd : StateController
	{
		public HitAdd(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_hits = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32? hits = EvaluationHelper.AsInt32(character, Hits, null);

			if (hits == null || hits <= 0) return;

            character.Team.Display.ComboCounter.AddHits(hits.Value);
		}

		public override Boolean IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Hits == null) return false;

			return true;
		}

		public Evaluation.Expression Hits
		{
			get { return m_hits; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Evaluation.Expression m_hits;

		#endregion
	}
}