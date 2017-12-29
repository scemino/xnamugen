using System;
using System.Diagnostics;

namespace xnaMugen.Events
{
	internal class FadeScreen : Base
	{
		public FadeScreen(FadeDirection direction)
		{
			if (direction == FadeDirection.None) throw new ArgumentNullException(nameof(direction));

			m_direction = direction;
		}

		public FadeDirection Direction => m_direction;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly FadeDirection m_direction;

		#endregion
	}
}