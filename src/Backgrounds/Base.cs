using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Collections.Generic;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using xnaMugen.Collections;

namespace xnaMugen.Backgrounds
{
	[DebuggerDisplay("{Name}")]
	abstract class Base
	{
		static Base()
		{
			s_titleregex = new Regex(".*BG\\s*(\\S.*)", RegexOptions.IgnoreCase);
		}

		protected Base(TextSection textsection)
		{
			if (textsection == null) throw new ArgumentNullException("textsection");

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

		public abstract void Update();

		public abstract void Draw(Combat.PaletteFx palettefx);

		protected void GetTileLength(Point size, out Point start, out Point end)
		{
			if (Tiling == new Point(0, 0))
			{
				start = new Point(0, 0);
				end = new Point(1, 1);

				return;
			}

			Point t = new Point();
			t.X = 1 + (Mugen.ScreenSize.X / size.X);
			t.Y = 1 + (Mugen.ScreenSize.Y / size.Y);

			start = new Point();
			end = new Point();

			if (Tiling.X == 0)
			{
				start.X = 0;
				end.X = 1;
			}
			else if (Tiling.X == 1)
			{
				start.X = -Math.Max(3, t.X);
				end.X = +Math.Max(3, t.X);
			}
			else
			{
				start.X = 0;
				end.X = Tiling.X;
			}

			if (Tiling.Y == 0)
			{
				start.Y = 0;
				end.Y = 1;
			}
			else if (Tiling.Y == 1)
			{
				start.Y = -Math.Max(3, t.Y);
				end.Y = +Math.Max(3, t.Y);
			}
			else
			{
				start.Y = 0;
				end.Y = Tiling.Y;
			}
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

		public Point Tiling
		{
			get { return m_tiling; }
		}

		public Point TilingSpacing
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

		#endregion
	}
}