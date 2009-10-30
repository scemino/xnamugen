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
	class SpriteManager
	{
		public SpriteManager(SpriteFile spritefile)
		{
			if (spritefile == null) throw new ArgumentNullException("spritefile");

			m_spritefile = spritefile;
			m_drawstate = new Video.DrawState(m_spritefile.SpriteSystem.GetSubSystem<Video.VideoSystem>());
		}

		public SpriteManager Clone()
		{
			SpriteManager clone = new SpriteManager(m_spritefile);
			clone.OverridePalette = OverridePalette;
			clone.UseOverride = UseOverride;

			return clone;
		}

		public void LoadAllSprites()
		{
			SpriteFile.LoadAllSprites();
		}

		public void LoadSprites(Animations.Animation animation)
		{
			if (animation == null) throw new ArgumentNullException("animation");

			foreach (Animations.AnimationElement element in animation)
			{
				GetSprite(element.SpriteId);
			}
		}

		public Sprite GetSprite(SpriteId spriteid)
		{
			return m_spritefile.GetSprite(spriteid);
		}

		public void Draw(SpriteId id, Vector2 location, Vector2 offset, Vector2 scale, SpriteEffects flip)
		{
			SetupDrawing(id, location, offset, scale, flip).Use();
		}

		public Video.DrawState SetupDrawing(SpriteId id, Vector2? location, Vector2 offset, Vector2 scale, SpriteEffects flip)
		{
			Sprite sprite = GetSprite(id);

			if ((flip & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally) offset.X = -offset.X;
			if ((flip & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically) offset.Y = -offset.Y;

			m_drawstate.Reset();
			m_drawstate.Set(sprite);
			m_drawstate.Offset = offset;
			m_drawstate.Flip = flip;
			m_drawstate.Scale = scale;

			if (location != null) m_drawstate.AddData(location.Value, null);

			if (UseOverride == true && sprite != null && sprite.PaletteOverride == true)
			{
				if (OverridePalette != null)
				{
					m_drawstate.Palette = OverridePalette;
				}
				else
				{
					Texture2D newpalette = SpriteFile.GetFirstPalette();
					if (newpalette != null) m_drawstate.Palette = newpalette;
				}
			}

			return m_drawstate;
		}

		public Video.DrawState DrawState
		{
			get { return m_drawstate; }
		}

		public Texture2D OverridePalette
		{
			get { return m_overridepalette; }

			set { m_overridepalette = value; }
		}

		public Boolean UseOverride
		{
			get { return m_useoverride; }

			set { m_useoverride = value; }
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
		Texture2D m_overridepalette;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_useoverride;

		#endregion
	}
}