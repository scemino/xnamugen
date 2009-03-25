using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("PosSet")]
	class PosSet : StateController
	{
		public PosSet(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_x = textsection.GetAttribute<Evaluation.Expression>("x", null);
			m_y = textsection.GetAttribute<Evaluation.Expression>("y", null);
		}

		public override void Run(Combat.Character character)
		{
			Single? x = EvaluationHelper.AsSingle(character, X, null);
			Single? y = EvaluationHelper.AsSingle(character, Y, null);

			Vector2 cameralocation = (Vector2)character.Engine.Camera.Location;

			Vector2 location = character.CurrentLocation;
			if (x != null) location.X = cameralocation.X + x.Value;
			if (y != null) location.Y = y.Value;

			character.CurrentLocation = location;
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