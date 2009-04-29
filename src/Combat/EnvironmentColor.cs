using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using xnaMugen.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Combat
{
	class EnvironmentColor : EngineObject
	{
		public EnvironmentColor(FightEngine fightengine)
			: base(fightengine)
		{
			m_color = Vector3.Zero;
			m_time = 0;
			m_under = false;
			m_hiddenlist = new List<Entity>();
			m_drawstate = new Video.DrawState(Engine.GetSubSystem<Video.VideoSystem>());
		}

		public void Update()
		{
			if (IsActive == true)
			{
				if (m_time > 0) --m_time;

				Hide();
			}
			else
			{
				Show();
			}
		}

		public void Draw()
		{
			m_drawstate.Reset();
			m_drawstate.AddData(Vector2.Zero, new Rectangle(0, 0, Mugen.ScreenSize.X, Mugen.ScreenSize.Y), new Color(Color));
			m_drawstate.Use();
		}

		public void Reset()
		{
			m_color = Vector3.Zero;
			m_time = 0;
			m_under = false;
			m_hiddenlist.Clear();
		}

		public void Setup(Vector3 color, Int32 time, Boolean under)
		{
			m_color = color;
			m_time = time;
			m_under = under;

			if (IsActive == true) Hide();
		}

		public Boolean IsHidden(Entity entity)
		{
			if (entity == null) throw new ArgumentNullException("entity");

			return IsActive && m_hiddenlist.Contains(entity);
		}

		void Show()
		{
			m_hiddenlist.Clear();
		}

		void Hide()
		{
			m_hiddenlist.Clear();

			if (UnderFlag == false)
			{
				foreach (Entity entity in Engine.Entities) m_hiddenlist.Add(entity);
			}
		}

		public Boolean IsActive
		{
			get { return m_time == -1 || m_time > 0; }
		}

		public Vector3 Color
		{
			get { return m_color; }
		}

		public Int32 Time
		{
			get { return m_time; }
		}

		public Boolean UnderFlag
		{
			get { return m_under; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector3 m_color;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_under;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Entity> m_hiddenlist;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Video.DrawState m_drawstate;

		#endregion
	}
}