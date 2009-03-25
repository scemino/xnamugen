using System;
using System.Diagnostics;

namespace xnaMugen.Events
{
	class SwitchScreen : Base
	{
		public SwitchScreen(ScreenType screen)
		{
			if (screen == ScreenType.None) throw new ArgumentNullException("screen");

			m_screen = screen;
		}

		public ScreenType Screen
		{
			get { return m_screen; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ScreenType m_screen;

		#endregion
	}
}