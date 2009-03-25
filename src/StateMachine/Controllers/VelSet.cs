using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("VelSet")]
	class VelSet : StateController
	{
		public VelSet(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_x = textsection.GetAttribute<Evaluation.Expression>("x", null);
			m_y = textsection.GetAttribute<Evaluation.Expression>("y", null);
		}

		public override void Run(Combat.Character character)
		{
			Single x = EvaluationHelper.AsSingle(character, X, character.CurrentVelocity.X);
			Single y = EvaluationHelper.AsSingle(character, Y, character.CurrentVelocity.Y);

			character.CurrentVelocity = new Vector2(x, y);
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