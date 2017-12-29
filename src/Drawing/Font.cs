using System;
using System.Diagnostics;
using xnaMugen.Collections;
using Microsoft.Xna.Framework;

namespace xnaMugen.Drawing
{
	[DebuggerDisplay("{" + nameof(Filepath) + "}")]
	internal class Font : Resource
	{
		public Font(SpriteSystem spritesystem, string filepath, Sprite sprite, ReadOnlyDictionary<char, Rectangle> sizemap, Point charsize, int colors)
		{
			if (spritesystem == null) throw new ArgumentNullException(nameof(spritesystem));
			if (filepath == null) throw new ArgumentNullException(nameof(filepath));
			if (sprite == null) throw new ArgumentNullException(nameof(sprite));
			if (sizemap == null) throw new ArgumentNullException(nameof(sizemap));

			m_spritesystem = spritesystem;
			m_filepath = filepath;
			m_sprite = sprite;
			m_sizemap = sizemap;
			m_charsize = charsize;
			m_colors = colors;
			m_drawstate = new Video.DrawState(m_spritesystem.GetSubSystem<Video.VideoSystem>());
		}

		public void Print(Vector2 location, int color, PrintJustification just, string text, Rectangle? scissorrect)
		{
			if (text == null) throw new ArgumentNullException(nameof(text));

			location = GetPrintLocation(text, location, just);

			m_drawstate.Reset();
			m_drawstate.Set(Sprite);
			m_drawstate.Mode = DrawMode.Font;
			m_drawstate.ScissorRectangle = scissorrect ?? Rectangle.Empty;
			m_drawstate.ShaderParameters.FontColorIndex = color;
			m_drawstate.ShaderParameters.FontTotalColors = m_colors;

			foreach (var c in text)
			{
				var r = GetCharRectangle(c);
				m_drawstate.AddData(location, r);

				location.X += r.Width;
			}

			m_drawstate.Use();
		}

		private Rectangle GetCharRectangle(char c)
		{
			var r = new Rectangle(0, 0, m_charsize.X, 0);

			if (c != ' ') m_sizemap.TryGetValue(c, out r);

			return r;
		}

		public int GetTextLength(string text)
		{
			if (text == null) throw new ArgumentNullException(nameof(text));

			var length = 0;
			foreach (var c in text)
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

		private Vector2 GetPrintLocation(string text, Vector2 location, PrintJustification just)
		{
			if (text == null) throw new ArgumentNullException(nameof(text));

			float length = GetTextLength(text);

			switch (just)
			{
				case PrintJustification.Center:
					location.X -= (int)(length / 2);
					break;

				case PrintJustification.Left:
					break;

				case PrintJustification.Right:
					location.X -= (int)length;
					break;
			}

			location.Y -= m_charsize.Y;

			return location;
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (m_sprite != null) m_sprite.Dispose();
			}

			base.Dispose(disposing);
		}

		public string Filepath => m_filepath;

		private Sprite Sprite => m_sprite;

		private ReadOnlyDictionary<char, Rectangle> SizeMap => m_sizemap;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_filepath;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Sprite m_sprite;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyDictionary<char, Rectangle> m_sizemap;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_charsize;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_colors;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SpriteSystem m_spritesystem;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Video.DrawState m_drawstate;

		#endregion
	}
}