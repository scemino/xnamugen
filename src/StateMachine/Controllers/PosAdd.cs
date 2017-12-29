using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("PosAdd")]
	internal class PosAdd : StateController
	{
		public PosAdd(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_x = textsection.GetAttribute<Evaluation.Expression>("x", null);
			m_y = textsection.GetAttribute<Evaluation.Expression>("y", null);
		}

		public override void Run(Combat.Character character)
		{
			var x = EvaluationHelper.AsSingle(character, X, 0);
			var y = EvaluationHelper.AsSingle(character, Y, 0);

			character.Move(new Vector2(x, y));
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