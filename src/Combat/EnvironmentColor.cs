using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace xnaMugen.Combat
{
	internal class EnvironmentColor : EngineObject
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
			if (IsActive)
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
			m_drawstate.Mode = DrawMode.FilledRectangle;
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

		public void Setup(Vector3 color, int time, bool under)
		{
			m_color = color;
			m_time = time;
			m_under = under;

			if (IsActive) Hide();
		}

		public bool IsHidden(Entity entity)
		{
			if (entity == null) throw new ArgumentNullException(nameof(entity));

			return IsActive && m_hiddenlist.Contains(entity);
		}

		private void Show()
		{
			m_hiddenlist.Clear();
		}

		private void Hide()
		{
			m_hiddenlist.Clear();

			if (UnderFlag == false)
			{
				foreach (var entity in Engine.Entities) m_hiddenlist.Add(entity);
			}
		}

		public bool IsActive => m_time == -1 || m_time > 0;

		public Vector3 Color => m_color;

		public int Time => m_time;

		public bool UnderFlag => m_under;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector3 m_color;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_under;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<Entity> m_hiddenlist;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Video.DrawState m_drawstate;

		#endregion
	}
}