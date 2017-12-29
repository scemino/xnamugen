using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.Backgrounds
{
	internal class Parallax : Base
	{
		public Parallax(TextSection textsection, Drawing.SpriteManager spritemanager)
			: base(textsection)
		{
			if (spritemanager == null) throw new ArgumentNullException(nameof(spritemanager));

			m_spritemanager = spritemanager;
			m_spriteid = textsection.GetAttribute("spriteno", SpriteId.Invalid);
			m_sprite = SpriteManager.GetSprite(SpriteId);
		}

		public override void Update()
		{
			Movement();
		}

		public override void Draw(Combat.PaletteFx palettefx)
		{
			GetTileLength(Sprite.Size, out var tilestart, out var tileend);

			var drawstate = SpriteManager.DrawState;
			drawstate.Reset();
			drawstate.Blending = Transparency;
			drawstate.ScissorRectangle = DrawRect;
			drawstate.Set(Sprite);

			for (var x = tilestart.X; x != tileend.X; ++x)
			{
				var adjustment = (Vector2)Sprite.Size * new Vector2(x, 0);
				var location = CurrentLocation + adjustment;

				drawstate.AddData(location, null);
			}

			palettefx?.SetShader(drawstate.ShaderParameters);

			drawstate.Use();
		}

		private void Movement()
		{
			var location = CurrentLocation + Velocity;

			if (Sprite != null)
			{
				var size = Sprite.Size;

				if (location.X >= StartLocation.X + size.X || location.X <= StartLocation.X - size.X) location.X = StartLocation.X;
				if (location.Y >= StartLocation.Y + size.Y || location.Y <= StartLocation.Y - size.Y) location.Y = StartLocation.Y;
			}

			CurrentLocation = location;
		}

		public Drawing.SpriteManager SpriteManager => m_spritemanager;

		public SpriteId SpriteId => m_spriteid;

		public Drawing.Sprite Sprite => m_sprite;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SpriteId m_spriteid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Drawing.Sprite m_sprite;

		#endregion
	}
}