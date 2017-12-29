using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace xnaMugen.Video
{
	[DebuggerDisplay("{Location} {DrawRect}")]
	internal struct DrawData
	{
		public DrawData(Vector2 location, Rectangle? rect)
		{
			m_location = location;
			m_drawrect = rect;
			m_tint = Color.White;
		}

		public DrawData(Vector2 location, Rectangle? rect, Color tint)
		{
			m_location = location;
			m_drawrect = rect;
			m_tint = tint;
		}

		public Vector2 Location => m_location;

		public Rectangle? DrawRect => m_drawrect;

		public Color Tint => m_tint;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_location;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Rectangle? m_drawrect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Color m_tint;

		#endregion
	}
}