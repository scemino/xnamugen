using System;
using xnaMugen.IO;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace xnaMugen.Replay
{
	class Recording
	{
		public Recording(Combat.EngineInitialization init, List<RecordingData> data)
		{
			if (init == null) throw new ArgumentNullException("init");
			if (data == null) throw new ArgumentNullException("data");

			m_initsettings = init;
			m_data = data;
		}

		public Combat.EngineInitialization InitializationSettings
		{
			get { return m_initsettings; }
		}

		public Collections.ListIterator<RecordingData> Data
		{
			get { return new Collections.ListIterator<RecordingData>(m_data); }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Combat.EngineInitialization m_initsettings;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<RecordingData> m_data;

		#endregion
	}
}