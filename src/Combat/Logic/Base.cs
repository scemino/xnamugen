using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat.Logic
{
	abstract class Base : EngineObject
	{
		protected Base(FightEngine engine, RoundState state)
			: base(engine)
		{
			if (state == RoundState.None) throw new ArgumentOutOfRangeException("state");

			m_tickcount = -1;
			m_state = state;
			m_element = null;
			m_text = null;
		}

		public virtual void Reset()
		{
			m_tickcount = -1;
			m_element = null;
			m_text = null;
		}

		public virtual void Update()
		{
			m_tickcount += 1;

			if (TickCount == 0)
			{
				OnFirstTick();
			}

			UpdateElement();
		}

		protected virtual void OnFirstTick()
		{
			m_element = GetElement();
			if(m_element != null) m_element.Reset();
		}

		protected abstract Elements.Base GetElement();

		public abstract Boolean IsFinished();

		void UpdateElement()
		{
			if (m_element == null) return;

			//m_element.Update();

			if (TickCount == m_element.DataMap.SoundTime)
			{
                if (m_element == Engine.Elements.GetElement("round.default"))
				{
					Engine.RoundInformation.GetRoundElement(Engine.RoundNumber).PlaySound();
				}
				else
				{
					m_element.PlaySound();
				}
			}

			if (m_element.FinishedDrawing(m_tickcount) == true)
			{
				m_element = null;
			}
		}

		public void Draw()
		{
			Vector2 location = Engine.RoundInformation.DefaultElementLocation;

			if (m_element != null)
			{
				if (m_element is Elements.Text && m_text != null)
				{
					Engine.Fonts.Print(m_element.DataMap.FontData, location + m_element.DataMap.Offset, m_text, null);
				}
				else
				{
					m_element.Draw(location);
				}
			}
		}

		public Int32 TickCount
		{
			get { return m_tickcount; }
		}

		public RoundState State
		{
			get { return m_state; }
		}

		public String DisplayString
		{
			get { return m_text; }

			set { m_text = value; }
		}

		public Elements.Base CurrentElement
		{
			get { return m_element; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly RoundState m_state;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_tickcount;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Elements.Base m_element;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		String m_text;

		#endregion
	}
}