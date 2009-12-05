using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xnaMugen.Collections;
using System.Collections;

namespace xnaMugen.Combat
{
	class EntityCollection : EngineObject, IEnumerable<Entity>
	{
		public struct Enumerator : IEnumerator<Entity>
		{
			public Enumerator(EntityCollection collection)
			{
				if (collection == null) throw new ArgumentNullException("collection");

				m_collection = collection;
				m_current = null;
				m_index = 0;
			}

			public Enumerator GetEnumerator()
			{
				return new Enumerator(m_collection);
			}

			public void Dispose()
			{
			}

			public Boolean MoveNext()
			{
				if (m_collection == null) return false;

				Int32 firstcount = m_collection.m_entities.Count;
				Int32 totalcount = firstcount + m_collection.m_addlist.Count;

				for (; m_index < totalcount; ++m_index)
				{
					m_current = (m_index < firstcount) ? m_collection.m_entities[m_index] : m_collection.m_addlist[m_index - firstcount];

					if (m_collection.m_removelist.Contains(m_current) == false)
					{
						++m_index;
						return true;
					}
				}

				m_current = null;
				return false;
			}

			public void Reset()
			{
				m_current = null;
				m_index = 0;
			}

			public Entity Current
			{
				get { return m_current; }
			}

			Object IEnumerator.Current
			{
				get { return m_current; }
			}

			#region Fields

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			readonly EntityCollection m_collection;

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			Entity m_current;

			[DebuggerBrowsable(DebuggerBrowsableState.Never)]
			Int32 m_index;

			#endregion
		}

		public EntityCollection(FightEngine fightengine)
			: base(fightengine)
		{
			m_entities = new List<Entity>();
			m_addlist = new List<Entity>();
			m_removelist = new List<Entity>();
			m_tempqueue = new List<Entity>();
			m_drawordercomparer = this.DrawOrderComparer;
			m_updateordercomparer = this.UpdateOrderComparer;
			m_removecheck = this.DrawRemoveCheck;
			m_inupdate = false;
		}

		public Boolean Contains(Entity entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");

			foreach (Entity e in this)
			{
				if (Object.ReferenceEquals(e, entity) == true) return true;
			}

			return false;
		}

		public void Add(Entity entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");
			if (Contains(entity) == true) throw new ArgumentException("Entity is already part of collection");

			if (m_inupdate == false)
			{
				m_entities.Add(entity);
			}
			else
			{
				m_addlist.Add(entity);
			}
		}

		public void Remove(Entity entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");
			if (Contains(entity) == false) throw new ArgumentException("Entity is not part of collection");
			if (m_removelist.Contains(entity) == true) throw new ArgumentException("Entity is already set to be removed from collection");

			if (m_inupdate == false)
			{
				m_entities.Remove(entity);
				m_addlist.Remove(entity);
			}
			else
			{
				m_removelist.Add(entity);
			}
		}

		public void Clear()
		{
			m_addlist.Clear();
			m_entities.Clear();
			m_removelist.Clear();
		}

		void AddEntities()
		{
			foreach (Entity entity in m_addlist)
			{
				m_entities.Add(entity);
			}

			m_addlist.Clear();
		}

		void RemoveEnities()
		{
			foreach (Entity entity in m_removelist)
			{
				m_entities.Remove(entity);
				m_addlist.Remove(entity);
			}

			m_removelist.Clear();
		}

		void RemoveCheck()
		{
			foreach (Entity entity in this)
			{
				if (entity.RemoveCheck() == true) Remove(entity);
			}
		}

		public void CountEntities(out Int32 players, out Int32 helpers, out Int32 explods, out Int32 projectiles)
		{
			players = 0;
			helpers = 0;
			explods = 0;
			projectiles = 0;

			foreach (Entity entity in this)
			{
				if (entity is Player) ++players;
				else if (entity is Helper) ++helpers;
				else if (entity is Explod) ++explods;
				else if (entity is Projectile) ++projectiles;
			}
		}

		public void Update(GameTime time)
		{
			m_inupdate = true;

			AddEntities();
			RemoveCheck();
			RemoveEnities();

			m_tempqueue.Clear();

			foreach (Entity entity in this)
			{
				if (Engine.SuperPause.IsPaused(entity) == true || Engine.Pause.IsPaused(entity) == true) continue;
				m_tempqueue.Add(entity);
			}

			RunEntityUpdates(m_tempqueue);
			m_tempqueue.Clear();

			while (m_addlist.Count > 0)
			{
				foreach (Entity entity in m_addlist)
				{
					if (Engine.SuperPause.IsPaused(entity) == true || Engine.Pause.IsPaused(entity) == true) continue;
					m_tempqueue.Add(entity);
				}

				AddEntities();

				RunEntityUpdates(m_tempqueue);
			}

			RemoveCheck();
			RemoveEnities();

			m_inupdate = false;
		}

		void RunEntityUpdates(List<Entity> entities)
		{
			if (entities == null) throw new ArgumentNullException("entities");

			entities.Sort(m_updateordercomparer);

			foreach (Entity entity in entities) entity.CleanUp();

			foreach (Entity entity in entities) entity.UpdateInput();

			foreach (Entity entity in entities) entity.UpdateAnimations();

			foreach (Entity entity in entities) entity.UpdateState();

            foreach (Entity entity in entities) entity.UpdateAfterImages();

			foreach (Entity entity in entities) entity.UpdatePhsyics();

			entities.Clear();
		}

		public void Draw(Boolean debug)
		{
			m_tempqueue.Clear();
			foreach (Entity entity in this)
			{
				if (entity.RemoveCheck() == true) continue;
				m_tempqueue.Add(entity);
			}

			m_tempqueue.Sort(m_drawordercomparer);

			Point savedcamerashift = Engine.GetSubSystem<Video.VideoSystem>().CameraShift;

			Engine.GetSubSystem<Video.VideoSystem>().CameraShift = Engine.Camera.Location * -1;

			foreach (Entity entity in m_tempqueue) entity.Draw();
			if (debug == true) foreach (Entity entity in m_tempqueue) entity.DebugDraw();

			Engine.GetSubSystem<Video.VideoSystem>().CameraShift = savedcamerashift;

			m_tempqueue.Clear();
		}

		public Enumerator GetEnumerator()
		{
			return new Enumerator(this);
		}

		IEnumerator<Entity> IEnumerable<Entity>.GetEnumerator()
		{
			return GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		Int32 UpdateOrderComparer(Entity lhs, Entity rhs)
		{
			if (lhs == null) throw new ArgumentNullException("lhs");
			if (rhs == null) throw new ArgumentNullException("rhs");

			if (lhs == rhs) return 0;

			Int32 order = lhs.UpdateOrder - rhs.UpdateOrder;
			if (order != 0) return order;

			if (lhs is Character && rhs is Character)
			{
				Character clhs = lhs as Character;
				Character crhs = rhs as Character;

				Boolean tlhs = clhs.MoveType == MoveType.BeingHit;
				Boolean trhs = crhs.MoveType == MoveType.BeingHit;

				if (tlhs == true && trhs == false)
				{
					return 1;
				}

				if (tlhs == false && trhs == true)
				{
					return -1;
				}
			}

			return m_entities.IndexOf(lhs) - m_entities.IndexOf(rhs);
		}

		Int32 DrawOrderComparer(Entity lhs, Entity rhs)
		{
			if (lhs == null) throw new ArgumentNullException("lhs");
			if (rhs == null) throw new ArgumentNullException("rhs");

			if (lhs == rhs) return 0;

			Int32 order = lhs.DrawOrder - rhs.DrawOrder;
			if (order != 0) return order;

			if (lhs is Player && (rhs is Player) == false) return 1;
			if (rhs is Player && (lhs is Player) == false) return -1;

			return m_entities.IndexOf(lhs) - m_entities.IndexOf(rhs);
		}

		Boolean DrawRemoveCheck(Entity entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");

			return m_removelist.Contains(entity) || Engine.EnvironmentColor.IsHidden(entity);
		}

		#region Fields

		readonly List<Entity> m_entities;

		readonly List<Entity> m_addlist;

		readonly List<Entity> m_removelist;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Entity> m_tempqueue;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Comparison<Entity> m_updateordercomparer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Comparison<Entity> m_drawordercomparer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Predicate<Entity> m_removecheck;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_inupdate;

		#endregion
	}
}