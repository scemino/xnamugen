using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using xnaMugen.Collections;

namespace xnaMugen.Video
{
	[DebuggerDisplay("{Location} {DrawRect}")]
	struct DrawData
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

		public Vector2 Location
		{
			get { return m_location; }
		}

		public Rectangle? DrawRect
		{
			get { return m_drawrect; }
		}

		public Color Tint
		{
			get { return m_tint; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_location;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Rectangle? m_drawrect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Color m_tint;

		#endregion
	}
}