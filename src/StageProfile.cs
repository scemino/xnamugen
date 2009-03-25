using System;
using System.Diagnostics;

namespace xnaMugen
{
	class StageProfile
	{
		public StageProfile(String filepath, String name)
		{
			if (filepath == null) throw new ArgumentNullException("filepath");
			if (name == null) throw new ArgumentNullException("name");

			m_filepath = filepath;
			m_name = name;
		}

		public override string ToString()
		{
			return Name;
		}

		public String Filepath
		{
			get { return m_filepath; }
		}

		public String Name
		{
			get { return m_name; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_filepath;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_name;

		#endregion
	}
}