using System;
using System.Diagnostics;
using xnaMugen.IO;

namespace xnaMugen.StateMachine.Controllers
{
	[StateControllerName("StateTypeSet")]
	class StateTypeSet : StateController
	{
		public StateTypeSet(StateSystem statesystem, String label, TextSection textsection)
			: base(statesystem, label, textsection)
		{
			m_statetype = textsection.GetAttribute<StateType>("statetype", StateType.Unchanged);
			m_movetype = textsection.GetAttribute<MoveType>("movetype", MoveType.Unchanged);
			m_physics = textsection.GetAttribute<Physics>("Physics", Physics.Unchanged);
		}

		public override void Run(Combat.Character character)
		{
			if (StateType != StateType.Unchanged && StateType != StateType.None) character.StateType = StateType;
			if (MoveType != MoveType.Unchanged && MoveType != MoveType.None) character.MoveType = MoveType;
			if (Physics != Physics.Unchanged) character.Physics = Physics;
		}

		public StateType StateType
		{
			get { return m_statetype; }
		}

		public MoveType MoveType
		{
			get { return m_movetype; }
		}

		public Physics Physics
		{
			get { return m_physics; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly StateType m_statetype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly MoveType m_movetype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Physics m_physics;

		#endregion
	}
}