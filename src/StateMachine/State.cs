using System;
using System.Diagnostics;
using xnaMugen.Collections;
using System.Collections.Generic;
using xnaMugen.IO;

namespace xnaMugen.StateMachine
{
	[DebuggerDisplay("State #{Number}, Controllers = {Controllers.Count}")]
	internal class State
	{
		public State(StateSystem statesystem, int number, TextSection textsection, List<StateController> controllers)
		{
			if (statesystem == null) throw new ArgumentNullException(nameof(statesystem));
			if (number < -3) throw new ArgumentOutOfRangeException(nameof(number), "State number must be greater then or equal to -3");
			if (textsection == null) throw new ArgumentNullException(nameof(textsection));
			if (controllers == null) throw new ArgumentNullException(nameof(controllers));

			m_statesystem = statesystem;
			m_number = number;
			m_controllers = new ReadOnlyList<StateController>(controllers);
			m_statetype = textsection.GetAttribute("type", StateType.Standing);
			m_movetype = textsection.GetAttribute("MoveType", MoveType.Idle);
			m_physics = textsection.GetAttribute("Physics", Physics.None);
			m_animationnumber = textsection.GetAttribute<Evaluation.Expression>("anim", null);
			m_velocity = textsection.GetAttribute<Evaluation.Expression>("velset", null);
			m_control = textsection.GetAttribute<Evaluation.Expression>("ctrl", null);
			m_power = textsection.GetAttribute<Evaluation.Expression>("poweradd", null);
			m_jugglepoints = textsection.GetAttribute<Evaluation.Expression>("juggle", null);
			m_faceenemy = textsection.GetAttribute<Evaluation.Expression>("facep2", null);
			m_hitdefpersist = textsection.GetAttribute<Evaluation.Expression>("hitdefpersist", null);
			m_movehitpersist = textsection.GetAttribute<Evaluation.Expression>("movehitpersist", null);
			m_hitcountpersist = textsection.GetAttribute<Evaluation.Expression>("hitcountpersist", null);
			m_spritepriority = textsection.GetAttribute<Evaluation.Expression>("sprpriority", null);
		}

		public StateSystem StateSystem => m_statesystem;

		public int Number => m_number;

		public StateType StateType => m_statetype;

		public MoveType MoveType => m_movetype;

		public Physics Physics => m_physics;

		public Evaluation.Expression AnimationNumber => m_animationnumber;

		public Evaluation.Expression Velocity => m_velocity;

		public Evaluation.Expression PlayerControl => m_control;

		public Evaluation.Expression Power => m_power;

		public Evaluation.Expression JugglePoints => m_jugglepoints;

		public Evaluation.Expression FaceEnemy => m_faceenemy;

		public Evaluation.Expression HitdefPersistance => m_hitdefpersist;

		public Evaluation.Expression MovehitPersistance => m_movehitpersist;

		public Evaluation.Expression HitCountPersistance => m_hitcountpersist;

		public Evaluation.Expression SpritePriority => m_spritepriority;

		public ReadOnlyList<StateController> Controllers => m_controllers;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly StateSystem m_statesystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_number;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly StateType m_statetype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly MoveType m_movetype;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Physics m_physics;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_animationnumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_velocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_control;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_power;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_jugglepoints;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_faceenemy;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_hitdefpersist;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_movehitpersist;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_hitcountpersist;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Evaluation.Expression m_spritepriority;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyList<StateController> m_controllers;

		#endregion
	}
}