using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AfterImageTime")]
	class AfterImageTime : StateController
	{
		public AfterImageTime(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			Evaluation.Expression exp_time = textsection.GetAttribute<Evaluation.Expression>("time", null);
			Evaluation.Expression exp_value = textsection.GetAttribute<Evaluation.Expression>("value", null);

			m_time = exp_time ?? exp_value;
		}

		public override void Run(Combat.Character character)
		{
			Int32? time = EvaluationHelper.AsInt32(character, DisplayTime, null);

			if (time != null)
			{
				character.AfterImages.ModifyDisplayTime(time.Value);
			}
			else
			{
				character.AfterImages.IsActive = false;
			}
		}

		public Evaluation.Expression DisplayTime
		{
			get { return m_time; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_time;

		#endregion
	}
}