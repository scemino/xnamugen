using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen
{
	class Timer
	{
		public Timer()
		{
			m_timespan = TimeSpan.Zero;
			m_running = false;
		}

		public virtual void Update(GameTime time)
		{
			if (IsRunning == true)
			{
				m_timespan += time.ElapsedGameTime;
			}
		}

		public virtual void Reset()
		{
			m_timespan = TimeSpan.Zero;
		}

		public Boolean IsRunning
		{
			get { return m_running; }

			set { m_running = value; }
		}

		public TimeSpan Time
		{
			get { return m_timespan; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_running;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		TimeSpan m_timespan;

		#endregion
	}

	class CountdownTimer : Timer
	{
		public CountdownTimer(TimeSpan time, EventHandler<EventArgs> IFunction)
		{
			if (time < new TimeSpan()) throw new ArgumentOutOfRangeException("Time cannot be negative");
			if (IFunction == null) throw new ArgumentNullException("IFunction");

			m_countdowntime = time;
			m_IFunction = IFunction;
		}

		public override void Update(GameTime time)
		{
			base.Update(time);

			if (IsRunning == true && m_wentoff == false && Time >= m_countdowntime)
			{
				m_wentoff = true;
				m_IFunction(this, EventArgs.Empty);
			}
		}

		public override void Reset()
		{
			base.Reset();

			m_wentoff = false;
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly TimeSpan m_countdowntime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_wentoff;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		EventHandler<EventArgs> m_IFunction;

		#endregion
	}
}
