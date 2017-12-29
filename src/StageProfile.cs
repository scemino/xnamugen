using System;
using System.Diagnostics;

namespace xnaMugen
{
	internal class StageProfile
	{
		public StageProfile(string filepath, string name)
		{
			if (filepath == null) throw new ArgumentNullException(nameof(filepath));
			if (name == null) throw new ArgumentNullException(nameof(name));

			m_filepath = filepath;
			m_name = name;
		}

		public override string ToString()
		{
			return Name;
		}

		public string Filepath => m_filepath;

		public string Name => m_name;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_filepath;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_name;

		#endregion
	}
}