using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("HitAdd")]
	internal class HitAdd : StateController
	{
		public HitAdd(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_hits = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			var hits = EvaluationHelper.AsInt32(character, Hits, null);

			if (hits == null || hits <= 0) return;

            character.Team.Display.ComboCounter.AddHits(hits.Value);
		}

		public override bool IsValid()
		{
			if (base.IsValid() == false) return false;

			if (Hits == null) return false;

			return true;
		}

		public Evaluation.Expression Hits => m_hits;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Evaluation.Expression m_hits;

		#endregion
	}
}