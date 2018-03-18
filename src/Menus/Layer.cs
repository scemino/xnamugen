using Microsoft.Xna.Framework;
using xnaMugen.Elements;

namespace xnaMugen.Menus
{
    internal class Layer
    {
        private bool m_enabled;

        public AnimatedImage AnimatedImage { get; set; }
        public int StartTime { get; set; }
        public Vector2 Offset { get; set; }

        public void Reset()
        {
            m_enabled = false;
            AnimatedImage?.Reset();
        }

        public void Draw(Vector2 position)
        {
            if (m_enabled)
            {
                AnimatedImage?.Draw(position);
            }
        }

        public void Update(int ticks)
        {
            if (ticks >= StartTime && !m_enabled)
            {
                m_enabled = true;
            }

            if (m_enabled)
            {
                AnimatedImage?.Update();
            }
        }
    }
}