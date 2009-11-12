using System;
using System.Diagnostics;

namespace xnaMugen.Events
{
	class LoadReplay : Base
	{
		public LoadReplay(Replay.Recording recording)
		{
			if (recording == null) throw new ArgumentNullException("recording");

			m_replay = recording;
		}

		public Replay.Recording Recording
		{
			get { return m_replay; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Replay.Recording m_replay;

		#endregion
	}
}