using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("Offset")]
	class Offset : StateController
	{
		public Offset(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_x = textsection.GetAttribute<Evaluation.Expression>("x", null);
			m_y = textsection.GetAttribute<Evaluation.Expression>("y", null);
		}

		public override void Run(Combat.Character character)
		{
			Single? x = EvaluationHelper.AsSingle(character, X, null);
			Single? y = EvaluationHelper.AsSingle(character, Y, null);

			Vector2 offset = character.DrawOffset;

			if (x != null) offset.X = x.Value;
			if (y != null) offset.Y = y.Value;

			character.DrawOffset = offset;
		}

		public Evaluation.Expression X
		{
			get { return m_x; }
		}

		public Evaluation.Expression Y
		{
			get { return m_y; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_x;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_y;

		#endregion
	}
}