using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace xnaMugen.StateMachine
{
	[AttributeUsage(AttributeTargets.Class)]
	internal class StateControllerNameAttribute : Attribute
	{
		public StateControllerNameAttribute(params string[] names)
		{
			if (names == null) throw new ArgumentNullException(nameof(names));
			if(names.Length == 0) throw new ArgumentException("names");

			m_names = new List<string>(names);
		}

		public Collections.ListIterator<string> Names => new Collections.ListIterator<string>(m_names);

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly List<string> m_names;

		#endregion
	}
}