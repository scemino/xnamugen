using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using System.Text.RegularExpressions;

namespace xnaMugen.Combat
{
	internal class BoundsRect
	{
		public BoundsRect(int left, int right, int top, int bottom)
		{
			m_left = left;
			m_right = right;
			m_top = top;
			m_bottom = bottom;
		}

		public Point Bound(Point input)
		{
			var output = input;

			if (output.X < Left) output.X = Left;
			if (output.X > Right) output.X = Right;

			if (output.Y < Top) output.Y = Top;
			if (output.Y > Bottom) output.Y = Bottom;

			return output;
		}

		public override string ToString()
		{
			return string.Format("Left: {0} Right: {1} Top: {2} Bottom {3}", Left, Right, Top, Bottom);
		}

		public int Left => m_left;

		public int Right => m_right;

		public int Top => m_top;

		public int Bottom => m_bottom;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_left;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_right;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_top;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_bottom;

		#endregion
	}

	[DebuggerDisplay("{" + nameof(Name) + "}")]
	internal class Stage : EngineObject
	{
		static Stage()
		{
			s_bgtitleregex = new Regex(@"bg[^(def)]", RegexOptions.IgnoreCase);
		}

		public Stage(FightEngine engine, StageProfile profile)
			: base(engine)
		{
			if (profile == null) throw new ArgumentNullException(nameof(profile));

			m_profile = profile;
			m_camerastartlocation = new Point(0, 0);
			m_p1start = new Vector2(0, 0);
			m_p2start = new Vector2(0, 0);
			m_palettefx = new PaletteFx();

			var textfile = Engine.GetSubSystem<FileSystem>().OpenTextFile(Profile.Filepath);
			var infosection = textfile.GetSection("Info");
			var camerasection = textfile.GetSection("Camera");
			var playerinfosection = textfile.GetSection("PlayerInfo");
			var boundsection = textfile.GetSection("Bound");
			var stageinfosection = textfile.GetSection("StageInfo");
			var shadowsection = textfile.GetSection("Shadow");
			var reflectionsection = textfile.GetSection("Reflection");
			var musicsection = textfile.GetSection("Music");
			var bgdefsection = textfile.GetSection("BGDef");

			if (infosection == null) throw new InvalidOperationException("Stage textfile '" + Profile.Filepath + "' is missing 'Info' section");
			if (camerasection == null) throw new InvalidOperationException("Stage textfile '" + Profile.Filepath + "' is missing 'Camera' section");
			if (playerinfosection == null) throw new InvalidOperationException("Stage textfile '" + Profile.Filepath + "' is missing 'PlayerInfo' section");
			if (boundsection == null) throw new InvalidOperationException("Stage textfile '" + Profile.Filepath + "' is missing 'Bound' section");
			if (stageinfosection == null) throw new InvalidOperationException("Stage textfile '" + Profile.Filepath + "' is missing 'StageInfo' section");
			if (shadowsection == null) throw new InvalidOperationException("Stage textfile '" + Profile.Filepath + "' is missing 'Shadow' section");
			//if (reflectionsection == null) throw new InvalidOperationException("Stage textfile '" + Profile.Filepath + "' is missing 'Reflection' section");
			if (musicsection == null) throw new InvalidOperationException("Stage textfile '" + Profile.Filepath + "' is missing 'Music' section");
			if (bgdefsection == null) throw new InvalidOperationException("Stage textfile '" + Profile.Filepath + "' is missing 'BGDef' section");

			m_name = infosection.GetAttribute<string>("name");

			m_camerastartlocation.X = camerasection.GetAttribute<int>("startx");
			m_camerastartlocation.Y = camerasection.GetAttribute<int>("starty");
			m_camerabounds = BuildBoundsRect(camerasection, "boundleft", "boundright", "boundhigh", "boundlow");
			m_floortension = camerasection.GetAttribute("floortension", 0);
			m_tension = camerasection.GetAttribute<int>("tension");
			m_verticalfollow = camerasection.GetAttribute<float>("verticalfollow");

			m_p1start.X = playerinfosection.GetAttribute<float>("p1startx");
			m_p1start.Y = playerinfosection.GetAttribute<float>("p1starty");
			m_p1facing = playerinfosection.GetAttribute<Facing>("p1facing");
			m_p2start.X = playerinfosection.GetAttribute<float>("p2startx");
			m_p2start.Y = playerinfosection.GetAttribute<float>("p2starty");
			m_p2facing = playerinfosection.GetAttribute<Facing>("p2facing");
            m_p3start.X = playerinfosection.GetAttribute("p3startx", m_p1start.X);
            m_p3start.Y = playerinfosection.GetAttribute("p3starty", m_p1start.Y);
            m_p3facing = playerinfosection.GetAttribute("p3facing", m_p1facing);
            m_p4start.X = playerinfosection.GetAttribute("p4startx", m_p2start.X);
            m_p4start.Y = playerinfosection.GetAttribute("p4starty", m_p2start.Y);
            m_p4facing = playerinfosection.GetAttribute("p4facing", m_p2facing);
			m_playerbounds = BuildBoundsRect(playerinfosection, "leftbound", "rightbound", "topbound", "botbound");

			m_screenleft = boundsection.GetAttribute<int>("screenleft");
			m_screenright = boundsection.GetAttribute<int>("screenright");

			m_zoffset = stageinfosection.GetAttribute<int>("zoffset");
			m_zoffsetlink = stageinfosection.GetAttribute<int?>("zoffsetlink", null);
			m_autoturn = stageinfosection.GetAttribute<bool>("autoturn");
			m_resetbg = stageinfosection.GetAttribute<bool>("resetBG");

			m_shadowintensity = stageinfosection.GetAttribute<byte>("intensity", 128);
			m_shadowcolor = stageinfosection.GetAttribute("color", Color.TransparentBlack);
			m_shadowscale = stageinfosection.GetAttribute("yscale", 0.4f);
			m_shadowfade = stageinfosection.GetAttribute<Point?>("fade.range", null);

			if (reflectionsection != null)
			{
				m_shadowreflection = reflectionsection.GetAttribute("reflect", false);
			}
			else
			{
				m_shadowreflection = false;
			}

			m_musicfile = musicsection.GetAttribute("bgmusic", string.Empty);
			m_volumeoffset = musicsection.GetAttribute("bgvolume", 0);

			m_spritefile = bgdefsection.GetAttribute<string>("spr");
			m_debug = bgdefsection.GetAttribute("debugbg", false);

			if (Engine.GetSubSystem<FileSystem>().DoesFileExist(m_spritefile) == false)
			{
				m_spritefile = Engine.GetSubSystem<FileSystem>().CombinePaths("stages", m_spritefile);
			}

			var spritemanager = Engine.GetSubSystem<Drawing.SpriteSystem>().CreateManager(SpritePath);
			var animationmanager = Engine.GetSubSystem<Animations.AnimationSystem>().CreateManager(Profile.Filepath);
			m_backgrounds = new Backgrounds.Collection(spritemanager, animationmanager);

			foreach (var textsection in textfile)
			{
				if (s_bgtitleregex.Match(textsection.Title).Success)
				{
					m_backgrounds.CreateBackground(textsection);
				}
			}

			Reset();
		}

		private static BoundsRect BuildBoundsRect(TextSection textsection, string left, string right, string top, string bottom)
		{
			var leftval = textsection.GetAttribute(left, 0);
			var rightval = textsection.GetAttribute(right, 0);
			var topval = textsection.GetAttribute(top, 0);
			var downval = textsection.GetAttribute(bottom, 0);

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
			var shift = new Point(Mugen.ScreenSize.X / 2 - Engine.Camera.Location.X, 0 - Engine.Camera.Location.Y);

			Engine.GetSubSystem<Video.VideoSystem>().CameraShift += shift;

			Backgrounds.Draw(layer, PaletteFx);

			Engine.GetSubSystem<Video.VideoSystem>().CameraShift -= shift;
		}

		public StageProfile Profile => m_profile;

		public string Name => m_name;

		public BoundsRect CameraBounds => m_camerabounds;

		public bool DebugBackgrounds => m_debug;

		public string SpritePath => m_spritefile;

		public string MusicFile => m_musicfile;

		public int MusicVolume => m_volumeoffset;

		public byte ShadowIntensity => m_shadowintensity;

		public Color ShadowColor => m_shadowcolor;

		public float ShadowScale => m_shadowscale;

		public Point? ShadowFade => m_shadowfade;

		public bool ShadowReflection => m_shadowreflection;

		public int ZOffset => m_zoffset;

		public int? ZOffsetLink => m_zoffsetlink;

		public bool AutoTurn => m_autoturn;

		public bool ResetBackgrounds => m_resetbg;

		public int LeftEdgeDistance => m_screenleft;

		public int RightEdgeDistance => m_screenright;

		public Vector2 P1Start => m_p1start;

		public Facing P1Facing => m_p1facing;

		public Vector2 P2Start => m_p2start;

		public Facing P2Facing => m_p2facing;

        public Vector2 P3Start => m_p3start;

        public Facing P3Facing => m_p3facing;

        public Vector2 P4Start => m_p4start;

        public Facing P4Facing => m_p4facing;

		public Point CameraStartLocation => m_camerastartlocation;

		public BoundsRect PlayerBounds => m_playerbounds;

		public float VerticalFollow => m_verticalfollow;

		public int Tension => m_tension;

		public int FloorTension => m_floortension;

		public Backgrounds.Collection Backgrounds => m_backgrounds;

		public PaletteFx PaletteFx => m_palettefx;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private static readonly Regex s_bgtitleregex;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Backgrounds.Collection m_backgrounds;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_name;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly BoundsRect m_camerabounds;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_spritefile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_debug;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly string m_musicfile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_volumeoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly byte m_shadowintensity;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Color m_shadowcolor;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly float m_shadowscale;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point? m_shadowfade;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_shadowreflection;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_zoffset;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int? m_zoffsetlink;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_autoturn;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_resetbg;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_screenleft;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_screenright;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_p1start;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Facing m_p1facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Vector2 m_p2start;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Facing m_p2facing;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Vector2 m_p3start;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Facing m_p3facing;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Vector2 m_p4start;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Facing m_p4facing;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_camerastartlocation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly BoundsRect m_playerbounds;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly float m_verticalfollow;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_floortension;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_tension;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly StageProfile m_profile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PaletteFx m_palettefx;

		#endregion
	}
}