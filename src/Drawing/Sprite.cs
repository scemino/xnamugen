using System;
using xnaMugen.IO;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.Collections;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.RegularExpressions;

namespace xnaMugen.Drawing
{
	class Sprite : Resource
	{
		public Sprite(Point size, Point axis, Texture2D pixels, Texture2D palette, Boolean paletteoverride)
		{
			if (pixels == null) throw new ArgumentNullException("pixels");
			if (palette == null) throw new ArgumentNullException("palette");

			m_size = size;
			m_axis = axis;
			m_pixels = pixels;
			m_palette = palette;
			m_paletteoverride = paletteoverride;
		}

		protected override void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
				if (m_pixels != null) m_pixels.Dispose();

				if (m_palette != null) m_palette.Dispose();
			}

			base.Dispose(disposing);
		}

		public Point Size
		{
			get { return m_size; }
		}

		public Point Axis
		{
			get { return m_axis; }
		}

		public Texture2D Pixels
		{
			get { return m_pixels; }
		}

		public Texture2D Palette
		{
			get { return m_palette; }
		}

		public Boolean PaletteOverride
		{
			get { return m_paletteoverride; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Point m_size;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Point m_axis;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Texture2D m_pixels;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Texture2D m_palette;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_paletteoverride;

		#endregion
	}
}