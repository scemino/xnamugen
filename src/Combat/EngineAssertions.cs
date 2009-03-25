using System;
using System.Diagnostics;

namespace xnaMugen.Combat
{
	class EngineAssertions
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

		public Boolean Intro
		{
			get { return m_intro; }

			set { m_intro = value; }
		}

		public Boolean WinPose
		{
			get { return m_winpose; }

			set { m_winpose = value; }
		}

		public Boolean NoBarDisplay
		{
			get { return m_nobardisplay; }

			set { m_nobardisplay = value; }
		}

		public Boolean NoBackLayer
		{
			get { return m_nobacklayer; }

			set { m_nobacklayer = value; }
		}

		public Boolean NoFrontLayer
		{
			get { return m_nofrontlayer; }

			set { m_nofrontlayer = value; }
		}

		public Boolean NoKOSound
		{
			get { return m_nokosound; }

			set { m_nokosound = value; }
		}

		public Boolean NoKOSlow
		{
			get { return m_nokoslow; }

			set { m_nokoslow = value; }
		}

		public Boolean NoGlobalShadow
		{
			get { return m_noglobalshadow; }

			set { m_noglobalshadow = value; }
		}

		public Boolean NoMusic
		{
			get { return m_nomusic; }

			set { m_nomusic = value; }
		}

		public Boolean TimerFreeze
		{
			get { return m_timerfreeze; }

			set { m_timerfreeze = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_intro;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_winpose;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_nobardisplay;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_nobacklayer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_nofrontlayer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_nokosound;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_nokoslow;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_noglobalshadow;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_nomusic;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_timerfreeze;

		#endregion
	}
}