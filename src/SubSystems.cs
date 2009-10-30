using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Reflection;

namespace xnaMugen
{
	abstract class SubSystem : Resource
	{
		protected SubSystem(SubSystems subsystems)
		{
			if (subsystems == null) throw new ArgumentNullException("subsystems");

			m_subsystems = subsystems;
		}

		public virtual void Initialize()
		{
		}

		public SubSystems SubSystems
		{
			get { return m_subsystems; }
		}

		public T GetSubSystem<T>() where T : SubSystem
		{
			return SubSystems.GetSubSystem<T>();
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SubSystems m_subsystems;

		#endregion
	}

	abstract class MainSystem : Resource
	{
		protected MainSystem(SubSystems subsystems)
		{
			if (subsystems == null) throw new ArgumentNullException("subsystems");

			m_subsystems = subsystems;
		}

		public SubSystems SubSystems
		{
			get { return m_subsystems; }
		}

		public T GetSubSystem<T>() where T : SubSystem
		{
			return SubSystems.GetSubSystem<T>();
		}

		public T GetMainSystem<T>() where T : MainSystem
		{
			return SubSystems.GetMainSystem<T>();
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SubSystems m_subsystems;

		#endregion
	}

	class SubSystems : Resource
	{
		public SubSystems(Game game)
		{
			if (game == null) throw new ArgumentNullException("game");

			m_game = game;
			m_subsystems = new Dictionary<Type, SubSystem>();
			m_mainsystems = new Dictionary<Type, MainSystem>();
		}

		protected override void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
				if (m_subsystems != null)
				{
					foreach (SubSystem subsystem in m_subsystems.Values) subsystem.Dispose();

					m_subsystems.Clear();
				}
			}

			base.Dispose(disposing);
		}

		public void LoadAllSubSystems()
		{
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.IsAbstract == true || type.IsSubclassOf(typeof(SubSystem)) == false) continue;

				if (m_subsystems.ContainsKey(type) == false) m_subsystems.Add(type, (SubSystem)Activator.CreateInstance(type, this));
			}
		}

		public void LoadAllMainSystems()
		{
			foreach (Type type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.IsAbstract == true || type.IsSubclassOf(typeof(MainSystem)) == false) continue;

				if (m_mainsystems.ContainsKey(type) == false) m_mainsystems.Add(type, (MainSystem)Activator.CreateInstance(type, this));
			}
		}

		public T GetSubSystem<T>() where T : SubSystem
		{
			Type type = typeof(T);

			if (m_subsystems.ContainsKey(type) == false) m_subsystems.Add(type, (SubSystem)Activator.CreateInstance(type, this));

			return m_subsystems[type] as T;
		}

		public T GetMainSystem<T>() where T : MainSystem
		{
			Type type = typeof(T);

			if (m_mainsystems.ContainsKey(type) == false) m_mainsystems.Add(type, (MainSystem)Activator.CreateInstance(type, this));

			return m_mainsystems[type] as T;
		}

		public Game Game
		{
			get { return m_game; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Game m_game;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Dictionary<Type, SubSystem> m_subsystems;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Dictionary<Type, MainSystem> m_mainsystems;

		#endregion
	}
}