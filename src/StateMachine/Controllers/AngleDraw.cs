using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("AngleDraw")]
	class AngleDraw : StateController
	{
		public AngleDraw(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_angle = textsection.GetAttribute<Evaluation.Expression>("value", null);
			m_scale = textsection.GetAttribute<Evaluation.Expression>("scale", null);
		}


		public override void Run(Combat.Character character)
		{
			Single angle = EvaluationHelper.AsSingle(character, Angle, character.DrawingAngle);

			character.DrawingAngle = angle;
			character.AngleDraw = true;

			Vector2? scale = EvaluationHelper.AsVector2(character, Scale, null);
			if (scale != null) character.DrawScale = scale.Value;
		}

		public Evaluation.Expression Angle
		{
			get { return m_angle; }
		}

		public Evaluation.Expression Scale
		{
			get { return m_scale; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_angle;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_scale;

		#endregion
	}
}