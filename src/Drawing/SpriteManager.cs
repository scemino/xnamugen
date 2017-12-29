using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Drawing
{
	[DebuggerDisplay("{SpriteFile.Filepath}")]
	internal class SpriteManager
	{
		public SpriteManager(SpriteFile spritefile)
		{
			if (spritefile == null) throw new ArgumentNullException(nameof(spritefile));

			m_spritefile = spritefile;
			m_drawstate = new Video.DrawState(m_spritefile.SpriteSystem.GetSubSystem<Video.VideoSystem>());
		}

		public SpriteManager Clone()
		{
			var clone = new SpriteManager(m_spritefile);
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
			if (animation == null) throw new ArgumentNullException(nameof(animation));

			foreach (var element in animation)
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

		public Video.DrawState SetupDrawing(SpriteId id, Vector2 location, Vector2 offset, Vector2 scale, SpriteEffects flip)
		{
			var sprite = GetSprite(id);

			if ((flip & SpriteEffects.FlipHorizontally) == SpriteEffects.FlipHorizontally) offset.X = -offset.X;
			if ((flip & SpriteEffects.FlipVertically) == SpriteEffects.FlipVertically) offset.Y = -offset.Y;

			m_drawstate.Reset();
			m_drawstate.Set(sprite);
			m_drawstate.Offset = offset;
			m_drawstate.AddData(location, null);
			m_drawstate.Flip = flip;
			m_drawstate.Scale = scale;

			if (UseOverride && sprite != null && sprite.PaletteOverride)
			{
				if (OverridePalette != null)
				{
					m_drawstate.Palette = OverridePalette;
				}
				else
				{
					var newpalette = SpriteFile.GetFirstPalette();
					if (newpalette != null) m_drawstate.Palette = newpalette;
				}
			}

			return m_drawstate;
		}

		public Video.DrawState DrawState => m_drawstate;

		public Texture2D OverridePalette
		{
			get => m_overridepalette;

			set { m_overridepalette = value; }
		}

		public bool UseOverride
		{
			get => m_useoverride;

			set { m_useoverride = value; }
		}

		private SpriteFile SpriteFile => m_spritefile;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SpriteFile m_spritefile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Video.DrawState m_drawstate;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Texture2D m_overridepalette;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_useoverride;

		#endregion
	}
}