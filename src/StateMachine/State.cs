using System;
using System.Diagnostics;
using xnaMugen.Collections;
using System.Collections.Generic;
using xnaMugen.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.Xna.Framework;

namespace xnaMugen.StateMachine
{
	[DebuggerDisplay("State #{Number}, Controllers = {Controllers.Count}")]
	class State
	{
		public State(StateSystem statesystem, Int32 number, TextSection textsection, List<StateController> controllers)
		{
			if (statesystem == null) throw new ArgumentNullException("statesystem");
			if (number < -3) throw new ArgumentOutOfRangeException("State number must be greater then or equal to -3");
			if (textsection == null) throw new ArgumentNullException("textsection");
			if (controllers == null) throw new ArgumentNullException("controllers");

			m_statesystem = statesystem;
			m_number = number;
			m_controllers = new ReadOnlyList<StateController>(controllers);
			m_statetype = textsection.GetAttribute<StateType>("type", StateType.Standing);
			m_movetype = textsection.GetAttribute<MoveType>("MoveType", MoveType.Idle);
			m_physics = textsection.GetAttribute<Physics>("Physics", Physics.None);
			m_animationnumber = textsection.GetAttribute<Evaluation.Expression>("anim", null);
			m_velocity = textsection.GetAttribute<Evaluation.Expression>("velset", null);
			m_control = textsection.GetAttribute<PlayerControl>("ctrl", PlayerControl.Unchanged);
			m_power = textsection.GetAttribute<Evaluation.Expression>("poweradd", null);
			m_jugglepoints = textsection.GetAttribute<Evaluation.Expression>("juggle", null);
			m_faceenemy = textsection.GetAttribute<Evaluation.Expression>("facep2", null);
			m_hitdefpersist = textsection.GetAttribute<Evaluation.Expression>("hitdefpersist", null);
			m_movehitpersist = textsection.GetAttribute<Evaluation.Expression>("movehitpersist", null);
			m_hitcountpersist = textsection.GetAttribute<Evaluation.Expression>("hitcountpersist", null);
			m_spritepriority = textsection.GetAttribute<Evaluation.Expression>("sprpriority", null);
		}

		public StateSystem StateSystem
		{
			get { return m_statesystem; }
		}

		public Int32 Number
		{
			get { return m_number; }
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

		public Evaluation.Expression AnimationNumber
		{
			get { return m_animationnumber; }
		}

		public Evaluation.Expression Velocity
		{
			get { return m_velocity; }
		}

		public PlayerControl PlayerControl
		{
			get { return m_control; }
		}

		public Evaluation.Expression Power
		{
			get { return m_power; }
		}

		public Evaluation.Expression JugglePoints
		{
			get { return m_jugglepoints; }
		}

		public Evaluation.Expression FaceEnemy
		{
			get { return m_faceenemy; }
		}

		public Evaluation.Expression HitdefPersistance
		{
			get { return m_hitdefpersist; }
		}

		public Evaluation.Expression MovehitPersistance
		{
			get { return m_movehitpersist; }
		}

		public Evaluation.Expression HitCountPersistance
		{
			get { return m_hitcountpersist; }
		}

		public Evaluation.Expression SpritePriority
		{
			get { return m_spritepriority; }
		}

		public ReadOnlyList<StateController> Controllers
		{
			get { return m_controllers; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly StateSystem m_statesystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_number;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly StateType m_statetype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly MoveType m_movetype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Physics m_physics;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_animationnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_velocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PlayerControl m_control;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_power;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_jugglepoints;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_faceenemy;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_hitdefpersist;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_movehitpersist;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_hitcountpersist;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Evaluation.Expression m_spritepriority;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyList<StateController> m_controllers;

		#endregion
	}
}