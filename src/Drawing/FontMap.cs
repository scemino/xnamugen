using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace xnaMugen.Drawing
{
	internal class FontMap : Resource
	{
		public FontMap(Dictionary<int, Font> fonts)
		{
			if (fonts == null) throw new ArgumentNullException(nameof(fonts));

			m_fonts = fonts;
		}

        public Font GetFont(int index)
        {
            if (index < 0) throw new ArgumentNullException(nameof(index));

	        if (m_fonts.TryGetValue(index, out var font) == false) return null;

            return font;
        }

		public void Print(PrintData data, Vector2 location, string text, Rectangle? scissor)
		{
			if (text == null) throw new ArgumentNullException(nameof(text));

			if (data.IsValid == false) return;

			if (m_fonts.TryGetValue(data.Index, out var font) == false) return;

			font.Print(location, data.ColorIndex, data.Justification, text, scissor);
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				if (m_fonts != null)
				{
					foreach (var font in m_fonts.Values)
					{
						font?.Dispose();
					}

					m_fonts.Clear();
				}
			}

			base.Dispose(disposing);
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
		private readonly Dictionary<int, Font> m_fonts;

		#endregion
	}
}