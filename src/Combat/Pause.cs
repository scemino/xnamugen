using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using xnaMugen.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Combat
{
	class Pause : EngineObject
	{
		public Pause(FightEngine fightengine, Boolean superpause)
			: base(fightengine)
		{
			m_issuperpause = superpause;
			m_creator = null;
			m_totaltime = 0;
			m_elapsedtime = -1;
			m_commandbuffertime = 0;
			m_movetime = 0;
			m_hitpause = false;
			m_pausebackgrounds = true;
			m_pausedentities = new List<Entity>();
		}

		public void Reset()
		{
			m_creator = null;
			m_totaltime = 0;
			m_elapsedtime = -1;
			m_commandbuffertime = 0;
			m_movetime = 0;
			m_hitpause = false;
			m_pausebackgrounds = true;
			m_pausedentities.Clear();
		}

		public void Update()
		{
			if (IsActive == true)
			{
				++m_elapsedtime;
			}
			else
			{
				Reset();
			}
		}

		public void Set(Character creator, Int32 time, Int32 buffertime, Int32 movetime, Boolean hitpause, Boolean pausebackgrounds)
		{
			if (creator == null) throw new ArgumentNullException("creator");

			Reset();

			m_creator = creator;
			m_totaltime = time;
			m_elapsedtime = 0;
			m_commandbuffertime = buffertime;
			m_movetime = movetime;
			m_hitpause = hitpause;
			m_pausebackgrounds = pausebackgrounds;

			foreach (Entity entity in Engine.Entities) m_pausedentities.Add(entity);
		}

		public Boolean IsPaused(Entity entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");

			if (m_pausedentities.Contains(entity) == false) return false;

			return entity.IsPaused(this);
		}

		public Boolean IsPaused(Stage stage)
		{
			if (stage == null) throw new ArgumentNullException("stage");

			if (IsActive == false) return false;

			return m_pausebackgrounds;
		}

		public Boolean IsActive
		{
			get { return m_elapsedtime >= 0 && m_elapsedtime <= m_totaltime; }
		}

		public Boolean IsSuperPause
		{
			get { return m_issuperpause; }
		}

		public Character Creator
		{
			get { return m_creator; }
		}

		public Int32 MoveTime
		{
			get { return m_movetime; }
		}

		public Int32 ElapsedTime
		{
			get { return m_elapsedtime; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_issuperpause;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Character m_creator;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_totaltime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_elapsedtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_commandbuffertime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_movetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_hitpause;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_pausebackgrounds;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Entity> m_pausedentities;

		#endregion
	}
}