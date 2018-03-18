using System;
using System.Diagnostics;

namespace xnaMugen.Combat.Logic
{
	internal abstract class Base : EngineObject
	{
		protected Base(FightEngine engine, RoundState state)
			: base(engine)
		{
			if (state == RoundState.None) throw new ArgumentOutOfRangeException(nameof(state));

			m_tickcount = -1;
			m_state = state;
			m_element = null;
			DisplayString = null;
		}

		public virtual void Reset()
		{
			m_tickcount = -1;
			m_element = null;
			DisplayString = null;
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
		}

		protected abstract Elements.Base GetElement();

		public abstract bool IsFinished();

		private void UpdateElement()
		{
			if (m_element == null) return;

			m_element.Update();

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

			if (m_element.FinishedDrawing(m_tickcount))
			{
				m_element = null;
			}
		}

		public void Draw()
		{
			var location = Engine.RoundInformation.DefaultElementLocation;

			if (m_element != null)
			{
				if (m_element is Elements.Text && DisplayString != null)
				{
					Engine.Fonts.Print(m_element.DataMap.FontData, location + m_element.DataMap.Offset, DisplayString, null);
				}
				else
				{
					m_element.Draw(location);
				}
			}
		}

		public int TickCount => m_tickcount;

		public RoundState State => m_state;

		public string DisplayString { get; set; }

		public Elements.Base CurrentElement => m_element;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly RoundState m_state;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_tickcount;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Elements.Base m_element;

		#endregion
	}
}