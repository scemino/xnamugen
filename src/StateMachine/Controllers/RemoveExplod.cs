using System;
using System.Diagnostics;
using xnaMugen.IO;

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

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Explod explod = character.FilterEntityAsExplod(entity, explod_id);
				if (explod == null) continue;

				explod.Kill();
			}
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