using System;
using System.Diagnostics;
using xnaMugen.IO;
using System.Collections.Generic;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("RemoveExplod")]
	class RemoveExplod : StateController
	{
		public RemoveExplod(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_id = textsection.GetAttribute<Evaluation.Expression>("ID", null);
		}

		public override void Run(Combat.Character character)
		{
			Int32 explod_id = EvaluationHelper.AsInt32(character, ExplodId, Int32.MinValue);

			List<Combat.Explod> removelist = new List<Combat.Explod>(character.GetExplods(explod_id));

			foreach (Combat.Explod explod in removelist) explod.Kill();
		}

		public Evaluation.Expression ExplodId
		{
			get { return m_id; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_id;

		#endregion
	}
}