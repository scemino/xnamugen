using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Drawing
{
	class Palette : Resource
	{
		public Palette(Texture2D texture, Byte[] colors)
		{
			if (texture == null) throw new ArgumentNullException("texture");
			if (colors == null) throw new ArgumentNullException("colors");

			m_texture = texture;
			m_basecolors = colors;
			m_colors = (Byte[])colors.Clone();
		}

		protected override void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
				if (m_texture != null) m_texture.Dispose();
			}

			base.Dispose(disposing);
		}

		public void Reset()
		{
			for (Int32 i = 0; i != m_basecolors.Length; ++i) m_colors[i] = m_basecolors[i];
		}

		public void SetTexture()
		{
			m_texture.SetData<Byte>(m_colors);
		}

		public Texture2D Texture
		{
			get { return m_texture; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Byte[] m_basecolors;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Byte[] m_colors;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Texture2D m_texture;

		#endregion
	}
}