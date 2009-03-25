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
	class FontMap : Resource
	{
		public FontMap(Dictionary<Int32, Font> fonts)
		{
			if (fonts == null) throw new ArgumentNullException("fonts");

			m_fonts = fonts;
		}

        public Font GetFont(Int32 index)
        {
            if (index < 0) throw new ArgumentNullException("index");

            Font font;
            if (m_fonts.TryGetValue(index, out font) == false) return null;

            return font;
        }

		public void Print(PrintData data, Vector2 location, String text, Rectangle? scissor)
		{
			if (text == null) throw new ArgumentNullException("text");

			if (data.IsValid == false) return;

			Font font;
			if (m_fonts.TryGetValue(data.Index, out font) == false) return;

			font.Print(location, data.ColorIndex, data.Justification, text, scissor);
		}

		protected override void Dispose(Boolean disposing)
		{
			if (disposing == true)
			{
				if (m_fonts != null)
				{
					foreach (Font font in m_fonts.Values) if (font != null) font.Dispose();
					m_fonts.Clear();
				}
			}

			base.Dispose(disposing);
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		readonly Dictionary<Int32, Font> m_fonts;

		#endregion
	}
}