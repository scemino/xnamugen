using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.StateMachine
{
	[AttributeUsage(AttributeTargets.Class)]
	class StateControllerNameAttribute : Attribute
	{
		public StateControllerNameAttribute(params String[] names)
		{
			if (names == null) throw new ArgumentNullException("names");
			if(names.Length == 0) throw new ArgumentException("names");

			m_names = new List<string>(names);
		}

		public Collections.ListIterator<String> Names
		{
			get { return new Collections.ListIterator<String>(m_names); }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<String> m_names;

		#endregion
	}
}