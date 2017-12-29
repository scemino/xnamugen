using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace xnaMugen.Combat
{
	internal class Pause : EngineObject
	{
		public Pause(FightEngine fightengine, bool superpause)
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
			if (IsActive)
			{
				++m_elapsedtime;
			}
			else
			{
				Reset();
			}
		}

		public void Set(Character creator, int time, int buffertime, int movetime, bool hitpause, bool pausebackgrounds)
		{
			if (creator == null) throw new ArgumentNullException(nameof(creator));

			Reset();

			m_creator = creator;
			m_totaltime = time;
			m_elapsedtime = 0;
			m_commandbuffertime = buffertime;
			m_movetime = movetime;
			m_hitpause = hitpause;
			m_pausebackgrounds = pausebackgrounds;

			foreach (var entity in Engine.Entities) m_pausedentities.Add(entity);
		}

		public bool IsPaused(Entity entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			if (m_pausedentities.Contains(entity) == false) return false;

			return entity.IsPaused(this);
		}

		public bool IsPaused(Stage stage)
		{
			if (stage == null) throw new ArgumentNullException(nameof(stage));

			if (IsActive == false) return false;

			return m_pausebackgrounds;
		}

		public bool IsActive => m_elapsedtime >= 0 && m_elapsedtime <= m_totaltime;

		public bool IsSuperPause => m_issuperpause;

		public Character Creator => m_creator;

		public int MoveTime => m_movetime;

		public int ElapsedTime => m_elapsedtime;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_issuperpause;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Character m_creator;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_totaltime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_elapsedtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_commandbuffertime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_movetime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_hitpause;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_pausebackgrounds;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<Entity> m_pausedentities;

		#endregion
	}
}