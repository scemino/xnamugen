using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("PosFreeze")]
	class PosFreeze : StateController
	{
		public PosFreeze(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_freeze = textsection.GetAttribute<Evaluation.Expression>("value", null);
		}

		public override void Run(Combat.Character character)
		{
			Boolean freeze = EvaluationHelper.AsBoolean(character, Freeze, true);

			character.PositionFreeze = freeze;
		}

		public Evaluation.Expression Freeze
		{
			get { return m_freeze; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_freeze;

		#endregion
	}
}