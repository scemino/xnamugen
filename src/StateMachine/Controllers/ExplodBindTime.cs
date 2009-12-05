using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("ExplodBindTime")]
	class ExplodBindTime : StateController
	{
		public ExplodBindTime(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_id = textsection.GetAttribute<Evaluation.Expression>("id", null);

			Evaluation.Expression exp_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			Evaluation.Expression exp_value = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_time = exp_time ?? exp_value;
		}

		public override void Run(Combat.Character character)
		{
			Int32 explod_id = EvaluationHelper.AsInt32(character, Id, Int32.MinValue);
			Int32 time = EvaluationHelper.AsInt32(character, Time, 1);

			foreach (Combat.Entity entity in character.Engine.Entities)
			{
				Combat.Explod explod = character.FilterEntityAsExplod(entity, explod_id);
				if (explod == null) continue;

				explod.Data.BindTime = time;
			}
		}

		public Evaluation.Expression Id
		{
			get { return m_id; }
		}

		public Evaluation.Expression Time
		{
			get { return m_time; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_id;

		#endregion
	}
}