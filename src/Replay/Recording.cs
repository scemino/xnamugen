using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.Replay
{
	internal class Recording
	{
		public Recording(Combat.EngineInitialization init, List<RecordingData> data)
		{
			if (init == null) throw new ArgumentNullException(nameof(init));
			if (data == null) throw new ArgumentNullException(nameof(data));

			m_initsettings = init;
			m_data = data;
		}

		public Combat.EngineInitialization InitializationSettings => m_initsettings;

		public Collections.ListIterator<RecordingData> Data => new Collections.ListIterator<RecordingData>(m_data);

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Combat.EngineInitialization m_initsettings;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<RecordingData> m_data;

		#endregion
	}
}