using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Drawing
{
    class Pixels
    {
        public Pixels(Point size, Byte[] pixels)
        {
            if (pixels == null) throw new ArgumentNullException("pixels");
            if (size.X <= 0 || size.Y <= 0) throw new ArgumentException("size");
            if (pixels.Length != size.X * size.Y) throw new ArgumentException("pixels");

            m_size = size;
            m_pixels = pixels;
        }

        public Point Size
        {
            get { return m_size; }
        }

        public Byte[] Buffer
        {
            get { return m_pixels; }
        }

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Point m_size;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Byte[] m_pixels;

        #endregion
    }
}