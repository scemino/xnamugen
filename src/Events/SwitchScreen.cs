using System;
using System.Diagnostics;

namespace xnaMugen.Events
{
	internal class SwitchScreen : Base
	{
		public SwitchScreen(ScreenType screen)
		{
			if (screen == ScreenType.None) throw new ArgumentNullException(nameof(screen));

			m_screen = screen;
		}

		public ScreenType Screen => m_screen;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ScreenType m_screen;

		#endregion
	}
}