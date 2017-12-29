using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AfterImageTime")]
	internal class AfterImageTime : StateController
	{
		public AfterImageTime(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			var expTime = textsection.GetAttribute<Evaluation.Expression>("time", null);
			var expValue = textsection.GetAttribute<Evaluation.Expression>("value", null);

			m_time = expTime ?? expValue;
		}

		public override void Run(Combat.Character character)
		{
			var time = EvaluationHelper.AsInt32(character, DisplayTime, null);

			if (time != null)
			{
				character.AfterImages.ModifyDisplayTime(time.Value);
			}
			else
			{
				character.AfterImages.IsActive = false;
			}
		}

		public Evaluation.Expression DisplayTime => m_time;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_time;

		#endregion
	}
}