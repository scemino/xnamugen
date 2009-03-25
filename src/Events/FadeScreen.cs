using System;
using System.Diagnostics;

namespace xnaMugen.Events
{
	class FadeScreen : Base
	{
		public FadeScreen(FadeDirection direction)
		{
			if (direction == FadeDirection.None) throw new ArgumentNullException("direction");

			m_direction = direction;
		}

		public FadeDirection Direction
		{
			get { return m_direction; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly FadeDirection m_direction;

		#endregion
	}
}