using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Drawing
{
    class Palette
    {
        public Palette(Color[] colors)
        {
            if (colors == null) throw new ArgumentNullException("colors");
            if (colors.Length != 256) throw new ArgumentException("colors");

            m_colors = colors;
        }

        public Palette()
            : this(new Color[256])
        {
        }

        public Color[] Buffer
        {
            get { return m_colors; }
        }

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.RootHidden)]
        readonly Color[] m_colors;

        #endregion
    }
}