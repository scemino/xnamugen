using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;

namespace xnaMugen.Combat
{
	class BoundsRect
	{
		public BoundsRect(Int32 left, Int32 right, Int32 top, Int32 bottom)
		{
			m_left = left;
			m_right = right;
			m_top = top;
			m_bottom = bottom;
		}

		public Point Bound(Point input)
		{
			Point output = input;

			if (output.X < Left) output.X = Left;
			if (output.X > Right) output.X = Right;

			if (output.Y < Top) output.Y = Top;
			if (output.Y > Bottom) output.Y = Bottom;

			return output;
		}

		public override String ToString()
		{
			return String.Format("Left: {0} Right: {1} Top: {2} Bottom {3}", Left, Right, Top, Bottom);
		}

		public Int32 Left
		{
			get { return m_left; }
		}

		public Int32 Right
		{
			get { return m_right; }
		}

		public Int32 Top
		{
			get { return m_top; }
		}

		public Int32 Bottom
		{
			get { return m_bottom; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_left;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_right;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_top;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_bottom;

		#endregion
	}

	[DebuggerDisplay("{Name}")]
	class Stage : EngineObject
	{
		static Stage()
		{
			s_bgtitleregex = new Regex(@"^bg[^(def)]", RegexOptions.IgnoreCase);
		}

		public Stage(FightEngine engine, StageProfile profile)
			: base(engine)
		{
			if (profile == null) throw new ArgumentNullException("profile");

			m_profile = profile;
			m_camerastartlocation = new Point(0, 0);
			m_p1start = new Vector2(0, 0);
			m_p2start = new Vector2(0, 0);
			m_palettefx = new PaletteFx();

			TextFile textfile = Engine.GetSubSystem<IO.FileSystem>().OpenTextFile(Profile.Filepath);
			TextSection infosection = textfile.GetSection("Info");
			TextSection camerasection = textfile.GetSection("Camera");
			TextSection playerinfosection = textfile.GetSection("PlayerInfo");
			TextSection scalingsection = textfile.GetSection("Scaling");
			TextSection boundsection = textfile.GetSection("Bound");
			TextSection stageinfosection = textfile.GetSection("StageInfo");
			TextSection shadowsection = textfile.GetSection("Shadow");
			TextSection reflectionsection = textfile.GetSection("Reflection");
			TextSection musicsection = textfile.GetSection("Music");
			TextSection bgdefsection = textfile.GetSection("BGDef");

			if (camerasection == null) throw new InvalidOperationException("Stage textfile '" + Profile.Filepath + "' is missing 'Camera' section");
			if (playerinfosection == null) throw new InvalidOperationException("Stage textfile '" + Profile.Filepath + "' is missing 'PlayerInfo' section");
			if (scalingsection == null) throw new InvalidOperationException("Stage textfile '" + Profile.Filepath + "' is missing 'Scaling' section");
			if (boundsection == null) throw new InvalidOperationException("Stage textfile '" + Profile.Filepath + "' is missing 'Bound' section");
			if (stageinfosection == null) throw new InvalidOperationException("Stage textfile '" + Profile.Filepath + "' is missing 'StageInfo' section");
			if (bgdefsection == null) throw new InvalidOperationException("Stage textfile '" + Profile.Filepath + "' is missing 'BGDef' section");

			if (infosection != null)
			{
				m_name = infosection.GetAttribute<String>("name");
			}
			else
			{
				m_name = Profile.Filepath;
			}

			m_camerastartlocation.X = camerasection.GetAttribute<Int32>("startx");
			m_camerastartlocation.Y = camerasection.GetAttribute<Int32>("starty");
			m_camerabounds = BuildBoundsRect(camerasection, "boundleft", "boundright", "boundhigh", "boundlow");
			m_floortension = camerasection.GetAttribute<Int32>("floortension", 0);
			m_tension = camerasection.GetAttribute<Int32>("tension");
			m_verticalfollow = camerasection.GetAttribute<Single>("verticalfollow");

			m_p1start.X = playerinfosection.GetAttribute<Single>("p1startx");
			m_p1start.Y = playerinfosection.GetAttribute<Single>("p1starty");
			m_p1facing = playerinfosection.GetAttribute<Facing>("p1facing");
			m_p2start.X = playerinfosection.GetAttribute<Single>("p2startx");
			m_p2start.Y = playerinfosection.GetAttribute<Single>("p2starty");
			m_p2facing = playerinfosection.GetAttribute<Facing>("p2facing");
			m_playerbounds = BuildBoundsRect(playerinfosection, "leftbound", "rightbound", "topbound", "botbound");

			m_screenleft = boundsection.GetAttribute<Int32>("screenleft");
			m_screenright = boundsection.GetAttribute<Int32>("screenright");

			m_zoffset = stageinfosection.GetAttribute<Int32>("zoffset");
			m_zoffsetlink = stageinfosection.GetAttribute<Int32?>("zoffsetlink", null);
			m_autoturn = stageinfosection.GetAttribute<Boolean>("autoturn");
			m_resetbg = stageinfosection.GetAttribute<Boolean>("resetBG", true);

			if (shadowsection != null)
			{
				m_shadowcolor = shadowsection.GetAttribute<Color>("color", new Color(127, 127, 127)).ToVector4();

				if (shadowsection.HasAttribute("intensity") == true)
				{
					Byte colorbyte = (Byte)Misc.Clamp(shadowsection.GetAttribute<Int32>("intensity", 127), 0, 255);
					m_shadowcolor = new Color(colorbyte, colorbyte, colorbyte).ToVector4();
				}

				m_shadowscale = shadowsection.GetAttribute<Single>("yscale", 0.4f);
				m_shadowfade = shadowsection.GetAttribute<Point?>("fade.range", null);
			}
			else
			{
				m_shadowcolor = new Vector4(0.5f, 0.5f, 0.5f, 1);
				m_shadowscale = 0.4f;
				m_shadowfade = null;
			}

			if (reflectionsection != null)
			{
				m_reflectionintensity = (Byte)Misc.Clamp(reflectionsection.GetAttribute<Int32>("intensity", 0), 0, 255);
			}
			else
			{
				m_reflectionintensity = 0;
			}

			if (musicsection != null)
			{
				m_musicfile = musicsection.GetAttribute<String>("bgmusic", String.Empty);
				m_volumeoffset = musicsection.GetAttribute<Int32>("bgvolume", 0);
			}
			else
			{
				m_musicfile = String.Empty;
				m_volumeoffset = 0;
			}

			m_spritefile = bgdefsection.GetAttribute<String>("spr");
			m_debug = bgdefsection.GetAttribute<Boolean>("debugbg", false);

			if (m_spritefile.StartsWith("stages/", StringComparison.OrdinalIgnoreCase) == false) m_spritefile = "stages/" + m_spritefile;

			Drawing.SpriteManager spritemanager = Engine.GetSubSystem<Drawing.SpriteSystem>().CreateManager(SpritePath);
			Animations.AnimationManager animationmanager = Engine.GetSubSystem<Animations.AnimationSystem>().CreateManager(Profile.Filepath);
			m_backgrounds = new Backgrounds.Collection(spritemanager, animationmanager);

			foreach (TextSection textsection in textfile)
			{
				if (s_bgtitleregex.Match(textsection.Title).Success == true)
				{
					m_backgrounds.CreateBackground(textsection);
				}
			}

			Reset();
		}

		static BoundsRect BuildBoundsRect(TextSection textsection, String left, String right, String top, String bottom)
		{
			Int32 leftval = textsection.GetAttribute<Int32>(left);
			Int32 rightval = textsection.GetAttribute<Int32>(right);
			Int32 topval = textsection.GetAttribute<Int32>(top);
			Int32 downval = textsection.GetAttribute<Int32>(bottom);

			return new BoundsRect(leftval, rightval, topval, downval);
		}

		public void Reset()
		{
			Backgrounds.Reset();
			PaletteFx.Reset();
		}

		public void Update(GameTime gametime)
		{
			Backgrounds.Update();
		}

		public void Draw(BackgroundLayer layer)
		{
			Video.VideoSystem videosystem = Engine.GetSubSystem<Video.VideoSystem>();
			Point savedcameralocation = videosystem.CameraShift;
			Point initialposition = videosystem.CameraShift + new Point(Mugen.ScreenSize.X / 2, 0);
			Vector2 cameralocation = (Vector2)Engine.Camera.Location;

			foreach (Backgrounds.Base background in Backgrounds)
			{
				if (background.IsVisible == false || background.Layer != layer) continue;

				videosystem.CameraShift = initialposition - (Point)(cameralocation * background.CameraDelta);

				background.Draw(PaletteFx);
			}

			videosystem.CameraShift = savedcameralocation;
		}

		public StageProfile Profile
		{
			get { return m_profile; }
		}

		public String Name
		{
			get { return m_name; }
		}

		public BoundsRect CameraBounds
		{
			get { return m_camerabounds; }
		}

		public Boolean DebugBackgrounds
		{
			get { return m_debug; }
		}

		public String SpritePath
		{
			get { return m_spritefile; }
		}

		public String MusicFile
		{
			get { return m_musicfile; }
		}

		public Int32 MusicVolume
		{
			get { return m_volumeoffset; }
		}

		public Vector4 ShadowColor
		{
			get { return m_shadowcolor; }
		}

		public Single ShadowScale
		{
			get { return m_shadowscale; }
		}

		public Point? ShadowFade
		{
			get { return m_shadowfade; }
		}

		public Byte ReflectionIntensity
		{
			get { return m_reflectionintensity; }
		}

		public Int32 ZOffset
		{
			get { return m_zoffset; }
		}

		public Int32? ZOffsetLink
		{
			get { return m_zoffsetlink; }
		}

		public Boolean AutoTurn
		{
			get { return m_autoturn; }
		}

		public Boolean ResetBackgrounds
		{
			get { return m_resetbg; }
		}

		public Int32 LeftEdgeDistance
		{
			get { return m_screenleft; }
		}

		public Int32 RightEdgeDistance
		{
			get { return m_screenright; }
		}

		public Vector2 P1Start
		{
			get { return m_p1start; }
		}

		public Facing P1Facing
		{
			get { return m_p1facing; }
		}

		public Vector2 P2Start
		{
			get { return m_p2start; }
		}

		public Facing P2Facing
		{
			get { return m_p2facing; }
		}

		public Point CameraStartLocation
		{
			get { return m_camerastartlocation; }
		}

		public BoundsRect PlayerBounds
		{
			get { return m_playerbounds; }
		}

		public Single VerticalFollow
		{
			get { return m_verticalfollow; }
		}

		public Int32 Tension
		{
			get { return m_tension; }
		}

		public Int32 FloorTension
		{
			get { return m_floortension; }
		}

		public Backgrounds.Collection Backgrounds
		{
			get { return m_backgrounds; }
		}

		public PaletteFx PaletteFx
		{
			get { return m_palettefx; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		static readonly Regex s_bgtitleregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Backgrounds.Collection m_backgrounds;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_name;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly BoundsRect m_camerabounds;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_spritefile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_debug;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly String m_musicfile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_volumeoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector4 m_shadowcolor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Single m_shadowscale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Point? m_shadowfade;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Byte m_reflectionintensity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_zoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32? m_zoffsetlink;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_autoturn;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Boolean m_resetbg;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_screenleft;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_screenright;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_p1start;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Facing m_p1facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Vector2 m_p2start;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Facing m_p2facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Point m_camerastartlocation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly BoundsRect m_playerbounds;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Single m_verticalfollow;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_floortension;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Int32 m_tension;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly StageProfile m_profile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PaletteFx m_palettefx;

		#endregion
	}
}