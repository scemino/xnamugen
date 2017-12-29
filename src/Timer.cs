using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen
{
	internal class Timer
	{
		public Timer()
		{
			m_timespan = TimeSpan.Zero;
			m_running = false;
		}

		public virtual void Update(GameTime time)
		{
			if (IsRunning)
			{
				m_timespan += time.ElapsedGameTime;
			}
		}

		public virtual void Reset()
		{
			m_timespan = TimeSpan.Zero;
		}

		public bool IsRunning
		{
			get => m_running;

			set { m_running = value; }
		}

		public TimeSpan Time => m_timespan;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_running;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private TimeSpan m_timespan;

		#endregion
	}

	internal class CountdownTimer : Timer
	{
		public CountdownTimer(TimeSpan time, EventHandler<EventArgs> function)
		{
			if (time < new TimeSpan()) throw new ArgumentOutOfRangeException(nameof(time), "Time cannot be negative");
			if (function == null) throw new ArgumentNullException(nameof(function));

			m_countdowntime = time;
			m_IFunction = function;
		}

		public override void Update(GameTime time)
		{
			base.Update(time);

			if (IsRunning && m_wentoff == false && Time >= m_countdowntime)
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
		private readonly TimeSpan m_countdowntime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_wentoff;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EventHandler<EventArgs> m_IFunction;

		#endregion
	}
}
