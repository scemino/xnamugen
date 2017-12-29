using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Combat
{
	internal class EnvironmentShake: EngineObject
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

		public void Set(int time, float frequency, int amplitude, float phase)
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

		public bool IsActive => m_time > 0 && m_timeticks < m_time;

		public Vector2 DrawOffset
		{
			get
			{
#warning Not yet done
				return new Vector2();

				if (IsActive == false) return new Vector2(0, 0);

				var movement = Amplitude * (float)Math.Sin(TimeElasped * Frequency + Phase);
				return new Vector2(movement, movement);
			}
		}

		public int TimeElasped => m_timeticks;

		public int Time => m_time;

		public float Frequency => m_frequency;

		public int Amplitude => m_amplitude;

		public float Phase => m_phase;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_timeticks;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_time;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private float m_frequency;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_amplitude;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private float m_phase;

		#endregion
	}
}