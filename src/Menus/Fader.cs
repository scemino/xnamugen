using xnaMugen.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace xnaMugen.Menus
{
    internal enum FaderState
    {
        None,
        FadeIn,
        FadeOut
    }

    internal class Fader
    {
        private int m_fadeTime;
        private readonly Texture2D m_emptyTexture;
        private FaderState m_state;

        public Color FadeInColor { get; private set; }
        public Color FadeOutColor { get; private set; }
        public int FadeInTime { get; private set; }
        public int FadeOutTime { get; private set; }

        public FaderState State
        {
            get { return m_state; }
            set
            {
                m_state = value;
                m_fadeTime = (m_state == FaderState.FadeIn) ? 255 : 0;
            }
        }

        public Fader(Texture2D emptyTexture, TextSection textSection)
        {
            if (textSection == null) throw new ArgumentNullException(nameof(textSection));

            m_emptyTexture = emptyTexture;
            m_fadeTime = 255;
            FadeInTime = textSection.GetAttribute<int>("fadein.time");
            FadeInColor = new Color(textSection.GetAttribute<Vector3>("fadein.col"));
            FadeOutTime = textSection.GetAttribute<int>("fadeout.time");
            FadeOutColor = new Color(textSection.GetAttribute<Vector3>("fadeout.col"));
        }

        public void Update()
        {
            switch (State)
            {
                case FaderState.FadeIn:
                    m_fadeTime -= (255 / (FadeInTime <= 0 ? 1 : FadeInTime));
                    if (m_fadeTime <= 0)
                    {
                        State = FaderState.None;
                    }
                    break;
                case FaderState.FadeOut:
                    m_fadeTime += (255 / (FadeOutTime <= 0 ? 1 : FadeOutTime));
                    if (m_fadeTime >= 255)
                    {
                        State = FaderState.None;
                    }
                    break;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            switch (State)
            {
                case FaderState.FadeIn:
                    {
                        var color = new Color(FadeInColor, m_fadeTime);
                        spriteBatch.Begin(blendState: BlendState.Additive);
                        spriteBatch.Draw(m_emptyTexture, new Rectangle(0, 0, Mugen.ScreenSize.X * 2, Mugen.ScreenSize.Y * 2), color);
                        spriteBatch.End();
                    }
                    break;
                case FaderState.FadeOut:
                    {
                        var color = new Color(FadeOutColor, m_fadeTime);
                        spriteBatch.Begin(blendState: BlendState.Additive);
                        spriteBatch.Draw(m_emptyTexture, new Rectangle(0, 0, Mugen.ScreenSize.X * 2, Mugen.ScreenSize.Y * 2), color);
                        spriteBatch.End();
                    }
                    break;
            }
        }
    }
}