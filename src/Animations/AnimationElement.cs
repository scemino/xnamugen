using System;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xnaMugen.IO;
using System.Text.RegularExpressions;

namespace xnaMugen.Animations
{
	/// <summary>
	/// Defines a single element of an Animation.
	/// </summary>
	class AnimationElement
	{
		/// <summary>
		/// Initializes a new instance of this class.
		/// </summary>
		/// <param name="id">The index of this element in the Animation.</param>
		/// <param name="clsns">The collection of collision boxes in this elements.</param>
		/// <param name="ticks">The time, in gameticks, that this element is drawn.</param>
		/// <param name="spriteid">The Sprite that is drawn for this element. SpriteId.Invalid for nothing to be drawn.</param>
		/// <param name="offset">The offset, in pixels, used to draw this element.</param>
		/// <param name="flip">The drawing orientation of this element.</param>
		/// <param name="blending">Alpha blending to be used while drawing this element.</param>
		public AnimationElement(Int32 id, List<Clsn> clsns, Int32 ticks, SpriteId spriteid, Point offset, SpriteEffects flip, Blending blending)
		{
			if (id < 0) throw new ArgumentOutOfRangeException("id");
			if (clsns == null) throw new ArgumentNullException("clsns");
			if (id < 0) throw new ArgumentOutOfRangeException("ticks");
			if (blending == null) throw new ArgumentNullException("blending");

			m_id = id;
			m_clsns = clsns;
			m_gameticks = ticks;
			m_spriteid = spriteid;
			m_offset = (Vector2)offset;
			m_flip = flip;
			m_blending = blending;
		}

		/// <summary>
		/// Return an enumerator used for iterating through the collision boxes of this element.
		/// </summary>
		/// <returns>An enumerator used for iterating through the collision boxes of this element.</returns>
		public List<Clsn>.Enumerator GetEnumerator()
		{
			return m_clsns.GetEnumerator();
		}

		/// <summary>
		/// Returns a string representation of this object.
		/// </summary>
		/// <returns>A string representation of this object.</returns>
		public override String ToString()
		{
			return "Element #" + Id.ToString();
		}

		/// <summary>
		/// Returns an iterator for iterating through the collision boxes of this element.
		/// </summary>
		/// <returns>An iterator for iterating through the collision boxes of this element.</returns>
		public ListIterator<Clsn> Clsns
		{
			get { return new ListIterator<Clsn>(m_clsns); }
		}

		/// <summary>
		/// Returns the index of this element in its containing Animation.
		/// </summary>
		/// <returns>The index of this element in its containing Animation.</returns>
		public Int32 Id
		{
			get { return m_id; }
		}

		/// <summary>
		/// Returns the time, in gameticks, that is element if drawn.
		/// </summary>
		/// <returns>The time, in gameticks, that is element if drawn.</returns>
		public Int32 Gameticks
		{
			get { return m_gameticks; }
		}

		/// <summary>
		/// Returns the SpriteId identifing the xnaMugen.Drawing.Sprite to be drawn for this element.
		/// </summary>
		/// <returns>The SpriteId identifing the xnaMugen.Drawing.Sprite to be drawn for this element.</returns>
		public SpriteId SpriteId
		{
			get { return m_spriteid; }
		}

		/// <summary>
		/// Returns the drawing offset of this element.
		/// </summary>
		/// <returns>The drawing offset of this element.</returns>
		public Vector2 Offset
		{
			get { return m_offset; }
		}

		/// <summary>
		/// Returns the drawing orientation of this element.
		/// </summary>
		/// <returns>The drawing orientation of this element.</returns>
		public SpriteEffects Flip
		{
			get { return m_flip; }
		}

		/// <summary>
		/// Returns the alpha blending used for drawing this element.
		/// </summary>
		/// <returns>The alpha blending used for drawing this element.</returns>
		public Blending Blending
		{
			get { return m_blending; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly List<Clsn> m_clsns;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_gameticks;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_offset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteId m_spriteid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteEffects m_flip;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Blending m_blending;

		#endregion
	}
}