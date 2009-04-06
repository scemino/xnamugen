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
	class Sprite
	{
		public Sprite(Point size, Point axis, Pixels pixels, Palette palette, Boolean paletteoverride)
		{
			if (pixels == null) throw new ArgumentNullException("pixels");
			if (palette == null) throw new ArgumentNullException("palette");

			m_size = size;
			m_axis = axis;
			m_pixels = pixels;
			m_palette = palette;
			m_paletteoverride = paletteoverride;
		}

		public Point Size
		{
			get { return m_size; }
		}

		public Point Axis
		{
			get { return m_axis; }
		}

        public Pixels Pixels
		{
			get { return m_pixels; }
		}

        public Palette Palette
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
        readonly Pixels m_pixels;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Palette m_palette;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_paletteoverride;

		#endregion
	}
}