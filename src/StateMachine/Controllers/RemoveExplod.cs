using System.Diagnostics;
using xnaMugen.IO;
using System.Collections.Generic;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("RemoveExplod")]
	internal class RemoveExplod : StateController
	{
		public RemoveExplod(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_id = textsection.GetAttribute<Evaluation.Expression>("ID", null);
		}

		public override void Run(Combat.Character character)
		{
			var explodId = EvaluationHelper.AsInt32(character, ExplodId, int.MinValue);

			var removelist = new List<Combat.Explod>(character.GetExplods(explodId));

			foreach (var explod in removelist) explod.Kill();
		}

		public Evaluation.Expression ExplodId => m_id;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_id;

		#endregion
	}
}