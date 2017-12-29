using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("PosSet")]
	internal class PosSet : StateController
	{
		public PosSet(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_x = textsection.GetAttribute<Evaluation.Expression>("x", null);
			m_y = textsection.GetAttribute<Evaluation.Expression>("y", null);
		}

		public override void Run(Combat.Character character)
		{
			var x = EvaluationHelper.AsSingle(character, X, null);
			var y = EvaluationHelper.AsSingle(character, Y, null);

			var cameralocation = (Vector2)character.Engine.Camera.Location;

			var location = character.CurrentLocation;
			if (x != null) location.X = cameralocation.X + x.Value;
			if (y != null) location.Y = y.Value;

			character.CurrentLocation = location;
		}

		public Evaluation.Expression X => m_x;

		public Evaluation.Expression Y => m_y;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_x;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_y;

		#endregion
	}
}