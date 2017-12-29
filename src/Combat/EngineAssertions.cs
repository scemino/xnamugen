using System.Diagnostics;

namespace xnaMugen.Combat
{
	internal class EngineAssertions
	{
		public EngineAssertions()
		{
			Reset();
		}

		public void Reset()
		{
			m_intro = false;
			m_winpose = false;
			m_nobardisplay = false;
			m_nobacklayer = false;
			m_nofrontlayer = false;
			m_nokosound = false;
			m_nokoslow = false;
			m_noglobalshadow = false;
			m_nomusic = false;
			m_timerfreeze = false;
		}

		public bool Intro
		{
			get => m_intro;

			set { m_intro = value; }
		}

		public bool WinPose
		{
			get => m_winpose;

			set { m_winpose = value; }
		}

		public bool NoBarDisplay
		{
			get => m_nobardisplay;

			set { m_nobardisplay = value; }
		}

		public bool NoBackLayer
		{
			get => m_nobacklayer;

			set { m_nobacklayer = value; }
		}

		public bool NoFrontLayer
		{
			get => m_nofrontlayer;

			set { m_nofrontlayer = value; }
		}

		public bool NoKOSound
		{
			get => m_nokosound;

			set { m_nokosound = value; }
		}

		public bool NoKOSlow
		{
			get => m_nokoslow;

			set { m_nokoslow = value; }
		}

		public bool NoGlobalShadow
		{
			get => m_noglobalshadow;

			set { m_noglobalshadow = value; }
		}

		public bool NoMusic
		{
			get => m_nomusic;

			set { m_nomusic = value; }
		}

		public bool TimerFreeze
		{
			get => m_timerfreeze;

			set { m_timerfreeze = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_intro;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_winpose;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_nobardisplay;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_nobacklayer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_nofrontlayer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_nokosound;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_nokoslow;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_noglobalshadow;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_nomusic;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_timerfreeze;

		#endregion
	}
}