using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
	class EnvironmentShake: EngineObject
	{
		public EnvironmentShake(FightEngine engine)
			: base(engine)
		{
		}

		public void Reset()
		{
			m_timeticks = 0;
			m_time = 0;
			m_frequency = 0;
			m_amplitude = 0;
			m_phase = 0;
		}

		public void Set(Int32 time, Single frequency, Int32 amplitude, Single phase)
		{
			m_timeticks = 0;
			m_time = time;
			m_frequency = frequency;
			m_amplitude = amplitude;
			m_phase = phase;
		}

		public void Update()
		{
			if (IsActive == false) return;

			++m_timeticks;
		}

		public Boolean IsActive
		{
			get { return m_time > 0 && m_timeticks < m_time; }
		}

		public Vector2 DrawOffset
		{
			get
			{
#warning Not yet done
				return new Vector2();

				if (IsActive == false) return new Vector2(0, 0);

				Single movement = Amplitude * (Single)Math.Sin((TimeElasped * Frequency) + Phase);
				return new Vector2(movement, movement);
			}
		}

		public Int32 TimeElasped
		{
			get { return m_timeticks; }
		}

		public Int32 Time
		{
			get { return m_time; }
		}

		public Single Frequency
		{
			get { return m_frequency; }
		}

		public Int32 Amplitude
		{
			get { return m_amplitude; }
		}

		public Single Phase
		{
			get { return m_phase; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_timeticks;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Single m_frequency;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_amplitude;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Single m_phase;

		#endregion
	}
}