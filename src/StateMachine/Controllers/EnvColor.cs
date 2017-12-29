using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("EnvColor")]
	internal class EnvColor : StateController
	{
		public EnvColor(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			var expValue = textsection.GetAttribute<Evaluation.Expression>("value", null);
			var expColor = textsection.GetAttribute<Evaluation.Expression>("Color", null);
			m_color = expValue ?? expColor;

			m_time = textsection.GetAttribute<Evaluation.Expression>("Time", null);
			m_under = textsection.GetAttribute<Evaluation.Expression>("under", null);
		}

		public override void Run(Combat.Character character)
		{
			var color = EvaluationHelper.AsVector3(character, ScreenColor, Vector3.One);
			var time = EvaluationHelper.AsInt32(character, Time, 1);
			var underflag = EvaluationHelper.AsBoolean(character, Under, false);

			character.Engine.EnvironmentColor.Setup(color, time, underflag);
		}

		public Evaluation.Expression ScreenColor => m_color;

		public Evaluation.Expression Time => m_time;

		public Evaluation.Expression Under => m_under;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_color;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_under;

		#endregion
	}
}