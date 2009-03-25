using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("MakeDust")]
	class MakeDust : StateController
	{
		public MakeDust(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_pos = textsection.GetAttribute<Evaluation.Expression>("pos", null);
			m_pos2 = textsection.GetAttribute<Evaluation.Expression>("pos2", null);
			m_spacing = textsection.GetAttribute<Evaluation.Expression>("spacing", null);
		}

		public override void Run(Combat.Character character)
		{
		}

		public Evaluation.Expression Position1
		{
			get { return m_pos; }
		}

		public Evaluation.Expression Position2
		{
			get { return m_pos2; }
		}

		public Evaluation.Expression Spacing
		{
			get { return m_spacing; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_pos;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_pos2;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_spacing;

		#endregion
	}
}