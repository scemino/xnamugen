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
    [DebuggerDisplay("{SpriteFile.Filepath}")]
    class SpriteManager : Resource
    {
        public SpriteManager(SpriteFile spritefile, Boolean useoverride)
        {
            if (spritefile == null) throw new ArgumentNullException("spritefile");

            m_spritefile = spritefile;
            m_drawstate = new Video.DrawState(m_spritefile.SpriteSystem.GetSubSystem<Video.VideoSystem>());
            m_useoverride = useoverride;
            m_textures = new Dictionary<SpriteId, Texture2D>();
            m_ischild = false;
        }

        SpriteManager(SpriteManager parent)
        {
            if (parent == null) throw new ArgumentNullException("parent");

            m_spritefile = parent.m_spritefile;
            m_drawstate = new Video.DrawState(m_spritefile.SpriteSystem.GetSubSystem<Video.VideoSystem>());
            m_useoverride = parent.m_useoverride;
            m_textures = parent.m_textures;
            m_overridepalette = parent.m_overridepalette;
            m_ischild = true;
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing == true)
            {
                if (m_ischild == false)
                {
                    if (m_textures != null)
                    {
                        foreach (Texture2D texture in m_textures.Values)
                        {
                            if (texture != null) texture.Dispose();
                        }

                        m_textures.Clear();
                    }
                }
            }

            base.Dispose(disposing);
        }

        public SpriteManager Clone()
        {
            return new SpriteManager(this);
        }

        public void LoadAllSprites()
        {
			foreach (SpriteId id in SpriteFile)
			{
				GetSprite(id);
				GetTexture(id);
			}
        }

        public void LoadSprites(Animations.Animation animation)
        {
            if (animation == null) throw new ArgumentNullException("animation");

            foreach (Animations.AnimationElement element in animation)
            {
                GetSprite(element.SpriteId);
                GetTexture(element.SpriteId);
            }
        }

        Texture2D GetTexture(SpriteId spriteid)
        {
            Texture2D texture = null;
            if (m_textures.TryGetValue(spriteid, out texture) == false)
            {
                Sprite sprite = GetSprite(spriteid);
                if (sprite == null) return null;

                texture = m_spritefile.SpriteSystem.GetSubSystem<Video.VideoSystem>().CreateTexture(sprite.Pixels, GetPalette(sprite));
                m_textures[spriteid] = texture;
            }

            return texture;
        }

        public Sprite GetSprite(SpriteId spriteid)
        {
            if (m_spritefile.ContainsSprite(spriteid) == false) return null;

            return m_spritefile.GetSprite(spriteid);
        }

        public void Draw(SpriteId id, Vector2? location, Vector2 offset, Vector2 scale, SpriteEffects flip)
        {
            SetupDrawing(id, location, offset, scale, flip).Use();
        }

        public Video.DrawState SetupDrawing(SpriteId id, Vector2? location, Vector2 offset, Vector2 scale, SpriteEffects flip)
        {
            if ((flip & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally) offset.X = -offset.X;
            if ((flip & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically) offset.Y = -offset.Y;

            m_drawstate.Reset();
            m_drawstate.Offset = offset;
            m_drawstate.Flip = flip;
            m_drawstate.Scale = scale;

            if (location != null) m_drawstate.AddData(location.Value, null);

            Sprite sprite = GetSprite(id);
            if (sprite != null)
            {
                m_drawstate.Axis = (Vector2)sprite.Axis;
                m_drawstate.Texture = GetTexture(id);
            }
            else
            {
                m_drawstate.Mode = DrawMode.None;
                m_drawstate.Texture = null;
                m_drawstate.Axis = Vector2.Zero;
            }

            return m_drawstate;
        }

        Palette GetPalette(Sprite sprite)
        {
            if (sprite == null) throw new ArgumentNullException("sprite");

            if (m_useoverride == true && sprite.PaletteOverride == true)
            {
                return OverridePalette ?? SpriteFile.GetFirstPalette();
            }
            else
            {
                return sprite.Palette;
            }
        }

        void ClearTextureCache()
        {
            foreach (Texture2D texture in m_textures.Values) texture.Dispose();

            m_textures.Clear();
        }

        public Video.DrawState DrawState
        {
            get { return m_drawstate; }
        }

        public Palette OverridePalette
        {
            get { return m_overridepalette; }

            set
            {
                if (m_ischild == true) throw new ArgumentException("Cannot change palette for child spritemanager.");

                if (m_overridepalette != value) ClearTextureCache();
                m_overridepalette = value;
            }
        }

        SpriteFile SpriteFile
        {
            get { return m_spritefile; }
        }

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly SpriteFile m_spritefile;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Video.DrawState m_drawstate;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Palette m_overridepalette;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Boolean m_useoverride;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Dictionary<SpriteId, Texture2D> m_textures;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Boolean m_ischild;

        #endregion
    }
}