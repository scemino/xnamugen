using System;
using System.Diagnostics;

namespace xnaMugen.Events
{
	internal class LoadReplay : Base
	{
		public LoadReplay(Replay.Recording recording)
		{
			if (recording == null) throw new ArgumentNullException(nameof(recording));

			m_replay = recording;
		}

		public Replay.Recording Recording => m_replay;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Replay.Recording m_replay;

		#endregion
	}
}