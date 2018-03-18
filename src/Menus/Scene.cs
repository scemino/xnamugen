using xnaMugen.IO;
using xnaMugen.Drawing;
using Microsoft.Xna.Framework;
using xnaMugen.Elements;
using xnaMugen.Animations;
using xnaMugen.Audio;
using xnaMugen.Video;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Menus
{
    internal class Scene
    {
        private int m_ticks;
        private int m_endTime;
        private readonly SpriteBatch m_spriteBatch;
        private readonly Layer[] m_layers;
        private readonly VideoSystem m_videoSystem;
        private readonly Fader m_fader;

        public Scene(MenuSystem menuSystem, TextSection textSection,
                     SpriteManager spriteManager, AnimationManager animationManager,
                     SoundManager soundManager, FontMap fontMap)
        {
            m_endTime = textSection.GetAttribute<int>("end.time");
            var clearColor = textSection.GetAttribute("clearcolor", (Vector3?)null);
            if (clearColor.HasValue) ClearColor = new Color(clearColor.Value);
            Position = textSection.GetAttribute("layerall.pos", (Vector2?)null);
            var collection = new Collection(spriteManager, animationManager, soundManager, fontMap);
            m_videoSystem = menuSystem.GetSubSystem<VideoSystem>();
            m_spriteBatch = new SpriteBatch(m_videoSystem.Device);

            m_fader = new Fader(m_videoSystem.EmptyTexture, textSection);
            m_layers = new Layer[10];
            for (var i = 0; i < m_layers.Length; i++)
            {
                m_layers[i] = new Layer();
                var prefix = $"layer{i}";
                if (textSection.HasAttribute($"{prefix}.starttime"))
                {
                    m_layers[i].StartTime = textSection.GetAttribute<int>($"{prefix}.starttime");
                }
                if (textSection.HasAttribute($"{prefix}.anim"))
                {
                    m_layers[i].AnimatedImage = (AnimatedImage)collection.Build(textSection, prefix);
                }
                if (textSection.HasAttribute($"{prefix}.offset"))
                {
                    m_layers[i].Offset = textSection.GetAttribute<Vector2>($"{prefix}.offset");
                }
            }
        }

        public Color? ClearColor { get; set; }
        public Vector2? Position { get; set; }

        public bool IsFinished => m_ticks >= m_endTime;

        public void Reset()
        {
            m_ticks = 0;
            m_fader.State = FaderState.FadeIn;
            // layers
            foreach (var layer in m_layers)
            {
                layer.Reset();
            }
        }

        public void Update()
        {
            m_fader.Update();
            foreach (var layer in m_layers)
            {
                layer.Update(m_ticks);
            }
            if (m_ticks == m_endTime - m_fader.FadeOutTime)
            {
                m_fader.State = FaderState.FadeOut;
            }
            m_ticks++;
        }

        public void Draw()
        {
            m_spriteBatch.Begin();
            m_spriteBatch.Draw(m_videoSystem.EmptyTexture, new Rectangle(0, 0, m_videoSystem.ScreenSize.X, m_videoSystem.ScreenSize.Y), ClearColor.GetValueOrDefault());
            m_spriteBatch.End();

            foreach (var layer in m_layers)
            {
                layer.Draw(Position.GetValueOrDefault());
            }

            m_fader.Draw(m_spriteBatch);
        }

    }
}