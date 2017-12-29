using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Reflection;

namespace xnaMugen
{
	internal abstract class SubSystem : Resource
	{
		protected SubSystem(SubSystems subsystems)
		{
			if (subsystems == null) throw new ArgumentNullException(nameof(subsystems));

			m_subsystems = subsystems;
		}

		public virtual void Initialize()
		{
		}

		public SubSystems SubSystems => m_subsystems;

		public T GetSubSystem<T>() where T : SubSystem
		{
			return SubSystems.GetSubSystem<T>();
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SubSystems m_subsystems;

		#endregion
	}

	internal abstract class MainSystem : Resource
	{
		protected MainSystem(SubSystems subsystems)
		{
			if (subsystems == null) throw new ArgumentNullException(nameof(subsystems));

			m_subsystems = subsystems;
		}

		public SubSystems SubSystems => m_subsystems;

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
		private readonly SubSystems m_subsystems;

		#endregion
	}

	internal class SubSystems : Resource
	{
		public SubSystems(Game game)
		{
			if (game == null) throw new ArgumentNullException(nameof(game));

			m_game = game;
			m_subsystems = new Dictionary<Type, SubSystem>();
			m_mainsystems = new Dictionary<Type, MainSystem>();
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (m_subsystems != null)
				{
					foreach (var subsystem in m_subsystems.Values) subsystem.Dispose();

					m_subsystems.Clear();
				}
			}

			base.Dispose(disposing);
		}

		public void LoadAllSubSystems()
		{
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.IsAbstract || type.IsSubclassOf(typeof(SubSystem)) == false || m_subsystems.ContainsKey(type)) continue;

				var constructor = ConstructorDelegate.FastConstruct(type, GetType());
				var subsystem = (SubSystem)constructor(this);

				m_subsystems.Add(type, subsystem);
			}
		}

		public void LoadAllMainSystems()
		{
			foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
			{
				if (type.IsAbstract || type.IsSubclassOf(typeof(MainSystem)) == false || m_mainsystems.ContainsKey(type)) continue;

				var constructor = ConstructorDelegate.FastConstruct(type, GetType());
				var mainsystem = (MainSystem)constructor(this);

				m_mainsystems.Add(type, mainsystem);
			}
		}

		public T GetSubSystem<T>() where T : SubSystem
		{
			var type = typeof(T);

			if (m_subsystems.ContainsKey(type) == false) m_subsystems.Add(type, (SubSystem)Activator.CreateInstance(type, this));

			return m_subsystems[type] as T;
		}

		public T GetMainSystem<T>() where T : MainSystem
		{
			var type = typeof(T);

			if (m_mainsystems.ContainsKey(type) == false) m_mainsystems.Add(type, (MainSystem)Activator.CreateInstance(type, this));

			return m_mainsystems[type] as T;
		}

		public Game Game => m_game;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Game m_game;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<Type, SubSystem> m_subsystems;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<Type, MainSystem> m_mainsystems;

		#endregion
	}
}