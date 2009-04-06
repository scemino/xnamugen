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
	[DebuggerDisplay("{Filepath}")]
	class Font : Resource
	{
        public Font(SpriteSystem spritesystem, String filepath, ReadOnlyList<Texture2D> textures, ReadOnlyDictionary<Char, Rectangle> sizemap, Point charsize, Int32 colors)
		{
			if (spritesystem == null) throw new ArgumentNullException("spritesystem");
			if (filepath == null) throw new ArgumentNullException("filepath");
            if (textures == null) throw new ArgumentNullException("textures");
			if (sizemap == null) throw new ArgumentNullException("sizemap");

			m_spritesystem = spritesystem;
			m_filepath = filepath;
            m_textures = textures;
			m_sizemap = sizemap;
			m_charsize = charsize;
			m_colors = colors;
			m_drawstate = new Video.DrawState(m_spritesystem.GetSubSystem<Video.VideoSystem>());
		}

        protected override void Dispose(Boolean disposing)
        {
            if (m_textures != null)
            {
                foreach (Texture2D texture in m_textures)
                {
                    if (texture != null) texture.Dispose();
                }
            }

            base.Dispose(disposing);
        }

		public void Print(Vector2 location, Int32 color, PrintJustification just, String text, Rectangle? scissorrect)
		{
			if (text == null) throw new ArgumentNullException("text");

			location = GetPrintLocation(text, location, just);

			m_drawstate.Reset();
            m_drawstate.Texture = m_textures[color];
			m_drawstate.Mode = DrawMode.Font;
			m_drawstate.ScissorRectangle = scissorrect ?? Rectangle.Empty;

			foreach (Char c in text)
			{
				Rectangle r = GetCharRectangle(c);
				m_drawstate.AddData(location, r);

				location.X += r.Width;
			}

			m_drawstate.Use();
		}

		Rectangle GetCharRectangle(Char c)
		{
			Rectangle r = new Rectangle(0, 0, m_charsize.X, 0);

			if (c != ' ') m_sizemap.TryGetValue(c, out r);

			return r;
		}

		public Int32 GetTextLength(String text)
		{
			if (text == null) throw new ArgumentNullException("text");

			Int32 length = 0;
			foreach (Char c in text)
			{
				if (c != ' ')
				{
					length += m_sizemap[c].Width;
				}
				else
				{
					length += m_charsize.X;
				}
			}

			return length;
		}

		Vector2 GetPrintLocation(String text, Vector2 location, PrintJustification just)
		{
			if (text == null) throw new ArgumentNullException("text");

			Single length = GetTextLength(text);

			switch (just)
			{
				case PrintJustification.Center:
					location.X -= (Int32)(length / 2);
					break;

				case PrintJustification.Left:
					break;

				case PrintJustification.Right:
					location.X -= (Int32)length;
					break;
			}

			location.Y -= m_charsize.Y;

			return location;
		}

		public String Filepath
		{
			get { return m_filepath; }
		}

		ReadOnlyDictionary<Char, Rectangle> SizeMap
		{
			get { return m_sizemap; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_filepath;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly ReadOnlyList<Texture2D> m_textures;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyDictionary<Char, Rectangle> m_sizemap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Point m_charsize;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_colors;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteSystem m_spritesystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Video.DrawState m_drawstate;

		#endregion
	}
}