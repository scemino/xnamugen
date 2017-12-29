using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("StateTypeSet")]
	internal class StateTypeSet : StateController
	{
		public StateTypeSet(StateSystem statesystem, string label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_statetype = textsection.GetAttribute("statetype", StateType.Unchanged);
			m_movetype = textsection.GetAttribute("movetype", MoveType.Unchanged);
			m_physics = textsection.GetAttribute("Physics", Physics.Unchanged);
		}

		public override void Run(Combat.Character character)
		{
			if (StateType != StateType.Unchanged && StateType != StateType.None) character.StateType = StateType;
			if (MoveType != MoveType.Unchanged && MoveType != MoveType.None) character.MoveType = MoveType;
			if (Physics != Physics.Unchanged) character.Physics = Physics;
		}

		public StateType StateType => m_statetype;

		public MoveType MoveType => m_movetype;

		public Physics Physics => m_physics;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly StateType m_statetype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly MoveType m_movetype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Physics m_physics;

		#endregion
	}
}