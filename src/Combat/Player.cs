using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using xnaMugen.Collections;
using System.Collections.Generic;
using System.Text;

namespace xnaMugen.Combat
{
	[DebuggerDisplay("Player - #{Id} - {Profile.PlayerName}")]
	class Player : Character
	{
		public Player(FightEngine engine, PlayerProfile profile, Team team)
			: base(engine)
		{
			if (profile == null) throw new ArgumentNullException("profile");
			if (team == null) throw new ArgumentNullException("team");

			m_profile = profile;
			m_spritemanager = Engine.GetSubSystem<Drawing.SpriteSystem>().CreateManager(Profile.SpritePath);
			m_animationmanager = Engine.GetSubSystem<Animations.AnimationSystem>().CreateManager(Profile.AnimationPath);
			m_soundmanager = Engine.GetSubSystem<Audio.SoundSystem>().CreateManager(Profile.SoundPath);
			m_statemanager = Engine.GetSubSystem<StateMachine.StateSystem>().CreateManager(this, Profile.StateFiles);
			m_commandmanager = Engine.GetSubSystem<Commands.CommandSystem>().CreateManager(Profile.CommandPath);
			m_constants = new PlayerConstants(this, Engine.GetSubSystem<IO.FileSystem>().OpenTextFile(Profile.ConstantsPath));
			m_dimensions = new CharacterDimensions(Constants);
			m_palettes = BuildPalettes();
			m_palettenumber = 0;
			m_power = 0;
			m_palfx = new PaletteFx();
			m_team = team;

			SpriteManager.UseOverride = true;

			if (Engine.GetSubSystem<InitializationSettings>().PreloadCharacterSprites == true)
			{
				SpriteManager.LoadAllSprites();
			}

			SetLocalAnimation(0, 0);

			CurrentScale = Constants.Scale;
			PushFlag = true;
            Life = Constants.MaximumLife;
            Power = 0;
		}

		public override void Reset()
		{
			base.Reset();

			PushFlag = true;
		}

		public override void CleanUp()
		{
			base.CleanUp();

			if (InHitPause == false)
			{
				PushFlag = true;
			}
		}

		public override Boolean RemoveCheck()
		{
			return false;
		}

		public override void RecieveInput(PlayerButton button, Boolean pressed)
		{
			base.RecieveInput(button, pressed);

			foreach (Entity entity in Engine.Entities)
			{
				Helper helper = entity as Helper;
				if (helper == null || helper.BasePlayer != this) continue;

				helper.RecieveInput(button, pressed);
			}
		}

		ReadOnlyList<Texture2D> BuildPalettes()
		{
			List<Texture2D> palettes = new List<Texture2D>(12);

			for (Int32 i = 0; i != 12; ++i)
			{
				String filepath = Profile.PaletteFiles[i];
				if (String.Equals(filepath, String.Empty) == true)
				{
					palettes.Add(Engine.GetSubSystem<Video.VideoSystem>().CreatePaletteTexture());
				}
				else
				{
					Texture2D palette = Engine.GetSubSystem<Drawing.SpriteSystem>().LoadPaletteFile(filepath);
					palettes.Add(palette);
				}
			}

			return new ReadOnlyList<Texture2D>(palettes);
		}

		public PlayerProfile Profile
		{
			get { return m_profile; }
		}

		public override Drawing.SpriteManager SpriteManager
		{
			get { return m_spritemanager; }
		}

		public override Animations.AnimationManager AnimationManager
		{
			get { return m_animationmanager; }
		}

		public override Audio.SoundManager SoundManager
		{
			get { return m_soundmanager; }
		}

		public override Commands.CommandManager CommandManager
		{
			get { return m_commandmanager; }
		}

		public override StateMachine.StateManager StateManager
		{
			get { return m_statemanager; }
		}

		public PlayerConstants Constants
		{
			get { return m_constants; }
		}

		public override CharacterDimensions Dimensions
		{
			get { return m_dimensions; }
		}

		public ReadOnlyList<Texture2D> Palettes
		{
			get { return m_palettes; }
		}

		public Int32 PaletteNumber
		{
			get { return m_palettenumber; }

			set 
			{ 
				m_palettenumber = value;

				SpriteManager.OverridePalette = Palettes[m_palettenumber];
			}
		}

		public Int32 Power
		{
			get { return m_power; }

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

		public override PaletteFx PaletteFx
		{
			get { return m_palfx; }
		}

		public override Team Team
		{
			get { return m_team; }
		}

		public override Player BasePlayer
		{
			get { return this; }
		}

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PlayerProfile m_profile;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Drawing.SpriteManager m_spritemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Animations.AnimationManager m_animationmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Audio.SoundManager m_soundmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Commands.CommandManager m_commandmanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PlayerConstants m_constants;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly CharacterDimensions m_dimensions;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly ReadOnlyList<Texture2D> m_palettes;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_palettenumber;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly StateMachine.StateManager m_statemanager;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		Int32 m_power;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly PaletteFx m_palfx;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Team m_team;

		#endregion
	}
}