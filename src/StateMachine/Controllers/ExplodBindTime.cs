using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("ExplodBindTime")]
	internal class ExplodBindTime : StateController
	{
		public ExplodBindTime(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_id = textsection.GetAttribute<Evaluation.Expression>("id", null);

			var expTime = textsection.GetAttribute<Evaluation.Expression>("time", null);
			var expValue = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_time = expTime ?? expValue;
		}

		public override void Run(Combat.Character character)
		{
			var explodId = EvaluationHelper.AsInt32(character, Id, int.MinValue);
			var time = EvaluationHelper.AsInt32(character, Time, 1);

			foreach (var explod in character.GetExplods(explodId)) explod.Data.BindTime = time;
		}

		public Evaluation.Expression Id => m_id;

		public Evaluation.Expression Time => m_time;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_id;

		#endregion
	}
}