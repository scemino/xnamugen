using System;
using System.Text.RegularExpressions;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.Backgrounds
{
	[DebuggerDisplay("{" + nameof(Name) + "}")]
	internal abstract class Base
	{
		static Base()
		{
			s_titleregex = new Regex(".*BG\\s*(\\S.*)", RegexOptions.IgnoreCase);
		}

		protected Base(TextSection textsection)
		{
			if (textsection == null) throw new ArgumentNullException(nameof(textsection));

			m_name = GetBackgroundName(textsection);
			m_id = textsection.GetAttribute("id", 0);
			m_startlocation = textsection.GetAttribute("start", Vector2.Zero);
			m_currentlocation = Vector2.Zero;
			m_delta = textsection.GetAttribute("delta", Vector2.Zero);
			m_tiling = textsection.GetAttribute("tile", new Point(0, 0));
			m_tilingspacing = textsection.GetAttribute("tilespacing", new Point(0, 0));
			m_velocity = textsection.GetAttribute("velocity", Vector2.Zero);
			m_masking = textsection.GetAttribute("masking", false);
			m_layer = textsection.GetAttribute("layerno", BackgroundLayer.Back);
			m_blending = textsection.GetAttribute("trans", new Blending());
			m_drawrect = textsection.GetAttribute("window", new Rectangle(0, 0, Mugen.ScreenSize.X, Mugen.ScreenSize.Y));
			m_paused = false;
			m_visible = true;
		}

		private static string GetBackgroundName(TextSection textsection)
		{
			if (textsection == null) throw new ArgumentNullException(nameof(textsection));

			var titlematch = s_titleregex.Match(textsection.Title);
			return titlematch.Success ? titlematch.Groups[1].Value : string.Empty;
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

			var t = new Point();
			t.X = 1 + Mugen.ScreenSize.X / size.X;
			t.Y = 1 + Mugen.ScreenSize.Y / size.Y;

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

		public string Name => m_name;

		public int Id => m_id;

		public Vector2 StartLocation => m_startlocation;

		public Vector2 CurrentLocation
		{
			get => m_currentlocation;

			set { m_currentlocation = value; }
		}

		public Vector2 CameraDelta => m_delta;

		public Point Tiling => m_tiling;

		public Point TilingSpacing => m_tilingspacing;

		public Vector2 Velocity => m_velocity;

		public bool Masking => m_masking;

		public BackgroundLayer Layer => m_layer;

		public Blending Transparency => m_blending;

		public Rectangle DrawRect => m_drawrect;

		public bool IsPaused
		{
			get => m_paused;

			set { m_paused = value; }
		}

		public bool IsVisible
		{
			get => m_visible;

			set { m_visible = value; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Regex s_titleregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_name;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_id;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_startlocation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private Vector2 m_currentlocation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_delta;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_tiling;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_tilingspacing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_velocity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_masking;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly BackgroundLayer m_layer;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Blending m_blending;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Rectangle m_drawrect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_paused;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_visible;

		#endregion
	}
}