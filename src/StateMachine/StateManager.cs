using System;
using System.Diagnostics;
using xnaMugen.Collections;
using System.Collections.Generic;

namespace xnaMugen.StateMachine
{
	internal class StateManager
	{
		public StateManager(StateSystem statesystem, Combat.Character character, ReadOnlyKeyedCollection<int, State> states)
		{
			if (statesystem == null) throw new ArgumentNullException(nameof(statesystem));
			if (character == null) throw new ArgumentNullException(nameof(character));
			if (states == null) throw new ArgumentNullException(nameof(states));

			m_statesystem = statesystem;
			m_character = character;
			m_states = states;
			m_persistencemap = new Dictionary<StateController, int>();
			m_foreignmanager = null;
			m_statetime = 0;

#if DEBUG
			m_stateorder = new CircularBuffer<State>(10);
#else
			m_currentstate = null;
			m_previousstate = null;
#endif
		}

		public void Clear()
		{
			m_foreignmanager = null;
			m_persistencemap.Clear();
			m_statetime = 0;

#if DEBUG
			m_stateorder.Clear();
#else
			m_currentstate = null;
			m_previousstate = null;
#endif
		}

		public StateManager Clone(Combat.Character character)
		{
			if (character == null) throw new ArgumentNullException(nameof(character));

			return new StateManager(StateSystem, character, States);
		}

		private void ApplyState(State state)
		{
			if (state == null) throw new ArgumentNullException(nameof(state));

			m_persistencemap.Clear();

			if (state.Physics != Physics.Unchanged) Character.Physics = state.Physics;
			if (state.StateType != StateType.Unchanged) Character.StateType = state.StateType;
			if (state.MoveType != MoveType.Unchanged) Character.MoveType = state.MoveType;

			var playercontrol = EvaluationHelper.AsInt32(m_character, state.PlayerControl, null);
			if (playercontrol != null) Character.PlayerControl = playercontrol > 0 ? PlayerControl.InControl : PlayerControl.NoControl;

			var animationnumber = EvaluationHelper.AsInt32(m_character, state.AnimationNumber, null);
			if (animationnumber != null) Character.SetLocalAnimation(animationnumber.Value, 0);

			var spritepriority = EvaluationHelper.AsInt32(m_character, state.SpritePriority, null);
			if (spritepriority != null) Character.DrawOrder = spritepriority.Value;

			var power = EvaluationHelper.AsInt32(m_character, state.Power, null);
			if (power != null) Character.BasePlayer.Power += power.Value;

			var velocity = EvaluationHelper.AsVector2(m_character, state.Velocity, null);
			if (velocity != null) Character.CurrentVelocity = velocity.Value;

			var hitdefpersistance = EvaluationHelper.AsBoolean(m_character, state.HitdefPersistance, false);
			if (hitdefpersistance == false)
			{
				Character.OffensiveInfo.ActiveHitDef = false;
				Character.OffensiveInfo.HitPauseTime = 0;
			}

			var movehitpersistance = EvaluationHelper.AsBoolean(m_character, state.MovehitPersistance, false);
			if (movehitpersistance == false)
			{
				Character.OffensiveInfo.MoveReversed = 0;
				Character.OffensiveInfo.MoveHit = 0;
				Character.OffensiveInfo.MoveGuarded = 0;
				Character.OffensiveInfo.MoveContact = 0;
			}

			var hitcountpersistance = EvaluationHelper.AsBoolean(m_character, state.HitCountPersistance, false);
			if (hitcountpersistance == false)
			{
				Character.OffensiveInfo.HitCount = 0;
				Character.OffensiveInfo.UniqueHitCount = 0;
			}
		}

		public bool ChangeState(int statenumber)
		{
			if (statenumber < 0) throw new ArgumentOutOfRangeException(nameof(statenumber), "Cannot change to state with number less than zero");

			var state = GetState(statenumber, false);

			if (state == null)
			{
				return false;
			}

#if DEBUG
			m_stateorder.Add(state);
#else
			m_previousstate = m_currentstate;
			m_currentstate = state;
#endif
			m_statetime = -1;
			return true;
		}

		private void RunCurrentStateLoop(bool hitpause)
		{
			while (true)
			{
				if (m_statetime == -1)
				{
					m_statetime = 0;
					ApplyState(CurrentState);
				}

				if (RunState(CurrentState, hitpause) == false) break;
			}
		}

		public void Run(bool hitpause)
		{
			if (Character is Combat.Helper)
			{
				if ((Character as Combat.Helper).Data.KeyControl)
				{
					RunState(-1, true, hitpause);
				}
			}
			else
			{
				if (ForeignManager == null) RunState(-3, true, hitpause);
				RunState(-2, true, hitpause);
				RunState(-1, true, hitpause);
			}

			RunCurrentStateLoop(hitpause);

			if (hitpause == false)
			{
				++m_statetime;
			}
		}

		private bool RunState(int statenumber, bool forcelocal, bool hitpause)
		{
			var state = GetState(statenumber, forcelocal);
			if (state != null) return RunState(state, hitpause);

			return false;
		}

		private State GetState(int statenumber, bool forcelocal)
		{
			if (ForeignManager != null && forcelocal == false) return ForeignManager.GetState(statenumber, true);

			return States.Contains(statenumber) ? States[statenumber] : null;
		}

		private bool RunState(State state, bool hitpause)
		{
			if (state == null) throw new ArgumentNullException(nameof(state));

			foreach (var controller in state.Controllers)
			{
				if (hitpause && controller.IgnoreHitPause == false) continue;

                var persistencecheck = state.Number < 0 || PersistenceCheck(controller, controller.Persistence);
				if (persistencecheck == false) continue;

				var triggercheck = controller.Triggers.Trigger(m_character);
				if (triggercheck == false) continue;

                if (controller.Persistence == 0 || controller.Persistence > 1) m_persistencemap[controller] = controller.Persistence;

				controller.Run(m_character);

				if (controller is Controllers.ChangeState || controller is Controllers.SelfState) return true;
			}

			return false;
		}

		private bool PersistenceCheck(StateController controller, int persistence)
		{
			if (controller == null) throw new ArgumentNullException(nameof(controller));

			if (m_persistencemap.ContainsKey(controller) == false) return true;

			if (persistence == 0)
			{
				return false;
			}

			if (persistence == 1)
			{
				return true;
			}

			if (persistence > 1)
			{
				m_persistencemap[controller] += -1;

				return m_persistencemap[controller] <= 0;
			}

			return false;
		}

		public StateSystem StateSystem => m_statesystem;

		public Combat.Character Character => m_character;

		private ReadOnlyKeyedCollection<int, State> States => m_states;

		public int StateTime => m_statetime;

		public State CurrentState
		{
			get 
			{
#if DEBUG
				return StateOrder.Size > 0 ? StateOrder.ReverseGet(0) : null; 
#else
				return m_currentstate;
#endif
			}
		}

		public State PreviousState
		{
			get
			{
#if DEBUG
				return StateOrder.Size > 1 ? StateOrder.ReverseGet(1) : null; 
#else
				return m_previousstate;
#endif
			}
		}

#if DEBUG
		private CircularBuffer<State> StateOrder => m_stateorder;
#endif

		public StateManager ForeignManager
		{
			get => m_foreignmanager;

			set { m_foreignmanager = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly StateSystem m_statesystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Combat.Character m_character;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyKeyedCollection<int, State> m_states;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<StateController, int> m_persistencemap;

#if DEBUG
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly CircularBuffer<State> m_stateorder;
#else
		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		State m_currentstate;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		State m_previousstate;
#endif

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private StateManager m_foreignmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_statetime;

		#endregion
	}
}