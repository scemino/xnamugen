using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("PosFreeze")]
	internal class PosFreeze : StateController
	{
		public PosFreeze(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_freeze = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			var freeze = EvaluationHelper.AsBoolean(character, Freeze, true);

			character.PositionFreeze = freeze;
		}

		public Evaluation.Expression Freeze => m_freeze;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_freeze;

		#endregion
	}
}