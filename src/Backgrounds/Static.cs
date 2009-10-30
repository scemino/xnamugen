using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using xnaMugen.Collections;

namespace xnaMugen.Backgrounds
{
	class Static : Base
	{
		public Static(TextSection textsection, Drawing.SpriteManager spritemanager)
			: base(textsection)
		{
			if (spritemanager == null) throw new ArgumentNullException("spritemanager");

			m_spritemanager = spritemanager;
			m_spriteid = textsection.GetAttribute<SpriteId>("spriteno", SpriteId.Invalid);
			m_sprite = SpriteManager.GetSprite(SpriteId);
		}

		public override void Update()
		{
			Movement();
		}

		public override void Draw(Combat.PaletteFx palettefx)
		{
			Point tilestart;
			Point tileend;
			GetTileLength(Sprite.Size, out tilestart, out tileend);

			Video.DrawState drawstate = SpriteManager.DrawState;
			drawstate.Reset();
			drawstate.Blending = Transparency;
			drawstate.ScissorRectangle = DrawRect;
			drawstate.Set(Sprite);

			for (Int32 y = tilestart.Y; y != tileend.Y; ++y)
			{
				for (Int32 x = tilestart.X; x != tileend.X; ++x)
				{
					Vector2 adjustment = (Vector2)(Sprite.Size + TilingSpacing) * new Vector2(x, y);
					Vector2 location = CurrentLocation + adjustment;

					drawstate.AddData(location, null);
				}
			}

			if (palettefx != null) palettefx.SetShader(drawstate.ShaderParameters);

			drawstate.Use();
		}

		void Movement()
		{
			Vector2 location = CurrentLocation + Velocity;

			if (Sprite != null)
			{
				Point size = Sprite.Size;

				if (location.X >= StartLocation.X + size.X || location.X <= StartLocation.X - size.X) location.X = StartLocation.X;
				if (location.Y >= StartLocation.Y + size.Y || location.Y <= StartLocation.Y - size.Y) location.Y = StartLocation.Y;
			}

			CurrentLocation = location;
		}

		public Drawing.SpriteManager SpriteManager
		{
			get { return m_spritemanager; }
		}

		public SpriteId SpriteId
		{
			get { return m_spriteid; }
		}

		public Drawing.Sprite Sprite
		{
			get { return m_sprite; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly SpriteId m_spriteid;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.Sprite m_sprite;

		#endregion
	}
}