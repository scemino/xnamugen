using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("EnvColor")]
	class EnvColor : StateController
	{
		public EnvColor(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			Evaluation.Expression exp_value = textsection.GetAttribute<Evaluation.Expression>("value", null);
			Evaluation.Expression exp_color = textsection.GetAttribute<Evaluation.Expression>("Color", null);
			m_color = exp_value ?? exp_color;

			m_time = textsection.GetAttribute<Evaluation.Expression>("Time", null);
			m_under = textsection.GetAttribute<Evaluation.Expression>("under", null);
		}

		public override void Run(Combat.Character character)
		{
			Vector3 color = EvaluationHelper.AsVector3(character, ScreenColor, Vector3.One);
			Int32 time = EvaluationHelper.AsInt32(character, Time, 1);
			Boolean underflag = EvaluationHelper.AsBoolean(character, Under, false);

			character.Engine.EnvironmentColor.Setup(color, time, underflag);
		}

		public Evaluation.Expression ScreenColor
		{
			get { return m_color; }
		}

		public Evaluation.Expression Time
		{
			get { return m_time; }
		}

		public Evaluation.Expression Under
		{
			get { return m_under; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_color;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_under;

		#endregion
	}
}