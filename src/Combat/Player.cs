using System;
using System.Diagnostics;
using Microsoft.Xna.Framework.Graphics;
using xnaMugen.Collections;
using System.Collections.Generic;

namespace xnaMugen.Combat
{
	[DebuggerDisplay("Player - #{Id} - {Profile.PlayerName}")]
	internal class Player : Character
	{
        public Player(FightEngine engine, PlayerProfile profile, PlayerMode mode, Team team)
			: base(engine)
		{
			if (profile == null) throw new ArgumentNullException(nameof(profile));
			if (team == null) throw new ArgumentNullException(nameof(team));

			m_profile = profile;
			m_spritemanager = Engine.GetSubSystem<Drawing.SpriteSystem>().CreateManager(Profile.SpritePath);
			m_animationmanager = Engine.GetSubSystem<Animations.AnimationSystem>().CreateManager(Profile.AnimationPath);
			m_soundmanager = Engine.GetSubSystem<Audio.SoundSystem>().CreateManager(Profile.SoundPath);
			m_statemanager = Engine.GetSubSystem<StateMachine.StateSystem>().CreateManager(this, Profile.StateFiles);

            m_commandmanager = Engine.GetSubSystem<Commands.CommandSystem>().CreateManager(mode, Profile.CommandPath);
			m_constants = new PlayerConstants(this, Engine.GetSubSystem<IO.FileSystem>().OpenTextFile(Profile.ConstantsPath));
			m_dimensions = new CharacterDimensions(Constants);
			m_palettes = BuildPalettes();
			m_palettenumber = 0;
			m_power = 0;
			m_palfx = new PaletteFx();
			m_team = team;
			m_helpers = new Dictionary<int, List<Helper>>();

			if (Engine.GetSubSystem<InitializationSettings>().PreloadCharacterSprites)
			{
				SpriteManager.LoadAllSprites();
			}

			SpriteManager.UseOverride = true;

			SetLocalAnimation(0, 0);

			CurrentScale = Constants.Scale;
			PushFlag = true;
		}

		public override void Reset()
		{
			base.Reset();

			PushFlag = true;
			m_helpers.Clear();
		}

		public override void CleanUp()
		{
			base.CleanUp();

			if (InHitPause == false)
			{
				PushFlag = true;
			}
		}

		public override bool RemoveCheck()
		{
			return false;
		}

		public override void RecieveInput(PlayerButton button, bool pressed)
		{
			base.RecieveInput(button, pressed);

			foreach (var entity in Engine.Entities)
			{
				var helper = entity as Helper;
				if (helper == null || helper.BasePlayer != this) continue;

				helper.RecieveInput(button, pressed);
			}
		}

		private ReadOnlyList<Texture2D> BuildPalettes()
		{
			var palettes = new List<Texture2D>(12);

			for (var i = 0; i != 12; ++i)
			{
				var filepath = Profile.PaletteFiles[i];
				if (string.Equals(filepath, string.Empty))
				{
					var palette = Engine.GetSubSystem<Video.VideoSystem>().CreatePaletteTexture();
					palettes.Add(palette);
				}
				else
				{
					var palette = Engine.GetSubSystem<Drawing.SpriteSystem>().LoadPaletteFile(filepath);
					palettes.Add(palette);
				}
			}

			return new ReadOnlyList<Texture2D>(palettes);
		}

		public PlayerProfile Profile => m_profile;

		public override Drawing.SpriteManager SpriteManager => m_spritemanager;

		public override Animations.AnimationManager AnimationManager => m_animationmanager;

		public override Audio.SoundManager SoundManager => m_soundmanager;

        public override Commands.ICommandManager CommandManager => m_commandmanager;

		public override StateMachine.StateManager StateManager => m_statemanager;

		public PlayerConstants Constants => m_constants;

		public override CharacterDimensions Dimensions => m_dimensions;

		public ReadOnlyList<Texture2D> Palettes => m_palettes;

		public int PaletteNumber
		{
			get => m_palettenumber;

			set 
			{ 
				m_palettenumber = value;
				CurrentPalette = Palettes[m_palettenumber];
			}
		}

		public int Power
		{
			get => m_power;

			set
			{
				value = Misc.Clamp(value, 0, Constants.MaximumPower);

				if (value > m_power)
				{
					if (m_power < 1000 && value >= 1000 && value < 2000) Engine.RoundInformation.PlaySoundElement("level1");
					if (m_power < 2000 && value >= 2000 && value < 3000) Engine.RoundInformation.PlaySoundElement("level2");
					if (m_power < 3000 && value >= 3000 && value < 4000) Engine.RoundInformation.PlaySoundElement("level3");
					if (m_power < 4000 && value >= 4000 && value < 5000) Engine.RoundInformation.PlaySoundElement("level4");
					if (m_power < 5000 && value >= 5000 && value < 6000) Engine.RoundInformation.PlaySoundElement("level5");
					if (m_power < 6000 && value >= 6000 && value < 7000) Engine.RoundInformation.PlaySoundElement("level6");
					if (m_power < 7000 && value >= 7000 && value < 8000) Engine.RoundInformation.PlaySoundElement("level7");
					if (m_power < 8000 && value >= 8000 && value < 9000) Engine.RoundInformation.PlaySoundElement("level8");
					if (m_power < 9000 && value >= 9000 && value < 10000) Engine.RoundInformation.PlaySoundElement("level9");
				}

				m_power = value;
			}
		}

		public override PaletteFx PaletteFx => m_palfx;

		public override Team Team => m_team;

		public override Player BasePlayer => this;

		public Dictionary<int, List<Helper>> Helpers => m_helpers;

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PlayerProfile m_profile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Animations.AnimationManager m_animationmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Audio.SoundManager m_soundmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Commands.ICommandManager m_commandmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PlayerConstants m_constants;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly CharacterDimensions m_dimensions;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly ReadOnlyList<Texture2D> m_palettes;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_palettenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly StateMachine.StateManager m_statemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_power;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PaletteFx m_palfx;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Team m_team;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Dictionary<int, List<Helper>> m_helpers;

		#endregion
	}
}