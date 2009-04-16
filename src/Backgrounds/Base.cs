using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using xnaMugen.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Backgrounds
{
	[DebuggerDisplay("{Name}")]
	abstract class Base
	{
		static Base()
		{
			s_titleregex = new Regex(".*BG\\s*(\\S.*)", RegexOptions.IgnoreCase);
		}

		protected Base(TextSection textsection, Drawing.SpriteManager spritemanager, Animations.AnimationManager animationmanager)
		{
			if (textsection == null) throw new ArgumentNullException("textsection");
			if (spritemanager == null) throw new ArgumentNullException("spritemanager");
			if (animationmanager == null) throw new ArgumentNullException("animationmanager");

			m_spritemanager = spritemanager;
			m_animationmanager = animationmanager;

			m_name = GetBackgroundName(textsection);
			m_id = textsection.GetAttribute<Int32>("id", 0);
			m_startlocation = textsection.GetAttribute<Vector2>("start", Vector2.Zero);
			m_currentlocation = Vector2.Zero;
			m_delta = textsection.GetAttribute<Vector2>("delta", Vector2.Zero);
			m_tiling = textsection.GetAttribute<Point>("tile", new Point(0, 0));
			m_tilingspacing = textsection.GetAttribute<Point>("tilespacing", new Point(0, 0));
			m_velocity = textsection.GetAttribute<Vector2>("velocity", Vector2.Zero);
			m_masking = textsection.GetAttribute<Boolean>("masking", false);
			m_layer = textsection.GetAttribute<BackgroundLayer>("layerno", BackgroundLayer.Back);
			m_blending = textsection.GetAttribute<Blending>("trans", new Blending());
			m_drawrect = textsection.GetAttribute<Rectangle>("window", new Rectangle(0, 0, Mugen.ScreenSize.X, Mugen.ScreenSize.Y));
			m_paused = false;
			m_visible = true;
		}

		static String GetBackgroundName(TextSection textsection)
		{
			if (textsection == null) throw new ArgumentNullException("textsection");

			Match titlematch = s_titleregex.Match(textsection.Title);
			return (titlematch.Success == true) ? titlematch.Groups[1].Value : String.Empty;
		}

		public virtual void Reset()
		{
			m_currentlocation = m_startlocation;
		}

		public virtual void Update()
		{
			AnimationManager.Update();
			DoMovement();
		}

		public void Draw(Combat.PaletteFx palettefx)
		{
			if (CurrentSprite == null) return;

			Video.DrawState drawstate = SpriteManager.SetupDrawing(DrawSpriteId, null, Vector2.Zero, Vector2.One, SpriteEffects.None);
			drawstate.Blending = Transparency;
			drawstate.ScissorRectangle = DrawRect;

			SetTiling(drawstate);

			if (palettefx != null) palettefx.SetShader(drawstate.ShaderParameters);

			drawstate.Use();
		}

		void SetTiling(Video.DrawState drawstate)
		{
			if (drawstate == null) throw new ArgumentNullException("drawstate");

			Point size = CurrentSprite.Size;

			Int32 startx, starty, endx, endy;
			if (this is Animated)
			{
				GetTileLength(TilingSpacing, Axis.X, out startx, out endx);
				GetTileLength(TilingSpacing, Axis.Y, out starty, out endy);
			}
			else
			{
				GetTileLength(size, Axis.X, out startx, out endx);
				GetTileLength(size, Axis.Y, out starty, out endy);
			}

			for (Int32 y = starty; y != endy; ++y)
			{
				for (Int32 x = startx; x != endx; ++x)
				{
					Vector2 adjustment;

					if (this is Animated)
					{
						adjustment = (Vector2)TilingSpacing * new Vector2(x, y);
					}
					else
					{
						adjustment = (Vector2)(size + TilingSpacing) * new Vector2(x, y);
					}

					Vector2 drawlocation = CurrentLocation + adjustment;

					drawstate.AddData(drawlocation, null);
				}
			}
		}

		void GetTileLength(Point tilesize, Axis axis, out Int32 start, out Int32 end)
		{
			start = 0;
			end = 0;

			Int32 tile;
			Int32 sizeval;

			switch (axis)
			{
				case Axis.X:
					tile = Tiling.X;
					sizeval = (tilesize.X != 0) ? 1 + (Mugen.ScreenSize.X / tilesize.X) : 1;
					break;

				case Axis.Y:
					tile = Tiling.Y;
					sizeval = (tilesize.Y != 0) ? 1 + (Mugen.ScreenSize.Y / tilesize.Y) : 1;
					break;

				default:
					return;
			}

			if (tile == 0)
			{
				start = 0;
				end = 1;
			}
			else if (tile == 1)
			{
				start = -Math.Max(3, sizeval);
				end = Math.Max(3, sizeval);
			}
			else if (tile > 1)
			{
				start = 0;
				end = tile;
			}
			else
			{
				start = 0;
				end = 0;
			}
		}

		void DoMovement()
		{
			Vector2 location = CurrentLocation + Velocity;

			if (CurrentSprite != null)
			{
				Point size;
				if (this is Animated)
				{
					size = TilingSpacing;
				}
				else
				{
					size = CurrentSprite.Size;
				}

				if (location.X >= StartLocation.X + size.X || location.X <= StartLocation.X - size.X) location.X = StartLocation.X;
				if (location.Y >= StartLocation.Y + size.Y || location.Y <= StartLocation.Y - size.Y) location.Y = StartLocation.Y;
			}

			CurrentLocation = location;
		}

		public Drawing.SpriteManager SpriteManager
		{
			get { return m_spritemanager; }
		}

		public Animations.AnimationManager AnimationManager
		{
			get { return m_animationmanager; }
		}

		public String Name
		{
			get { return m_name; }
		}

		public Int32 Id
		{
			get { return m_id; }
		}

		public Vector2 StartLocation
		{
			get { return m_startlocation; }
		}

		public Vector2 CurrentLocation
		{
			get { return m_currentlocation; }

			set { m_currentlocation = value; }
		}

		public Vector2 CameraDelta
		{
			get { return m_delta; }
		}

		public virtual Point Tiling
		{
			get { return m_tiling; }
		}

		public virtual Point TilingSpacing
		{
			get { return m_tilingspacing; }
		}

		public Vector2 Velocity
		{
			get { return m_velocity; }
		}

		public Boolean Masking
		{
			get { return m_masking; }
		}

		public BackgroundLayer Layer
		{
			get { return m_layer; }
		}

		public Blending Transparency
		{
			get { return m_blending; }
		}

		public Rectangle DrawRect
		{
			get { return m_drawrect; }
		}

		public Boolean IsPaused
		{
			get { return m_paused; }

			set { m_paused = value; }
		}

		public Boolean IsVisible
		{
			get { return m_visible; }

			set { m_visible = value; }
		}

		protected abstract SpriteId DrawSpriteId { get; }

		protected Drawing.Sprite CurrentSprite
		{
			get { return SpriteManager.GetSprite(DrawSpriteId); }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		static readonly Regex s_titleregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_name;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_startlocation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Vector2 m_currentlocation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_delta;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Point m_tiling;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Point m_tilingspacing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_velocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_masking;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly BackgroundLayer m_layer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Blending m_blending;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Rectangle m_drawrect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_paused;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Boolean m_visible;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Animations.AnimationManager m_animationmanager;

		#endregion
	}
}