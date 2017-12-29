using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Drawing
{
	internal class Sprite : Resource
	{
		public Sprite(Point size, Point axis, bool ownpixels, Texture2D pixels, bool ownpalette, Texture2D palette, bool paletteoverride)
		{
			if (pixels == null) throw new ArgumentNullException(nameof(pixels));
			if (palette == null) throw new ArgumentNullException(nameof(palette));

			m_size = size;
			m_axis = axis;
			m_pixels = pixels;
			m_palette = palette;
			m_paletteoverride = paletteoverride;
			m_ownpixels = ownpixels;
			m_ownpalette = ownpalette;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (m_ownpixels && m_pixels != null) m_pixels.Dispose();

				if (m_ownpalette && m_palette != null) m_palette.Dispose();
			}

			base.Dispose(disposing);
		}

		public Point Size => m_size;

		public Point Axis => m_axis;

		public Texture2D Pixels => m_pixels;

		public Texture2D Palette => m_palette;

		public bool PaletteOverride => m_paletteoverride;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_size;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_axis;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Texture2D m_pixels;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Texture2D m_palette;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_paletteoverride;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_ownpixels;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_ownpalette;

		#endregion
	}
}