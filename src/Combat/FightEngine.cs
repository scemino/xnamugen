using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using xnaMugen.Collections;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Combat
{
    class FightEngine : MainSystem
    {
        public FightEngine(SubSystems subsystems)
            : base(subsystems)
        {
            IO.TextFile textfile = GetSubSystem<IO.FileSystem>().OpenTextFile(@"data/fight.def");
            IO.TextSection filesection = textfile.GetSection("Files");
            String basepath = GetSubSystem<IO.FileSystem>().GetDirectory(textfile.Filepath);

            m_entities = new EntityCollection(this);
            m_roundnumber = 0;
            m_stage = null;
            m_idcounter = 0;
            m_tickcount = 0;
            m_pause = new Pause(this, false);
            m_superpause = new Pause(this, true);
            m_asserts = new EngineAssertions();
            m_camera = new Camera(this);
            m_envcolor = new EnvironmentColor(this);
            m_envshake = new EnvironmentShake(this);
            m_speed = GameSpeed.Normal;
            m_slowspeedbuffer = 0;
            m_fontmap = BuildFontMap(filesection);
            m_fightsounds = GetSubSystem<Audio.SoundSystem>().CreateManager(BuildPath(basepath, filesection.GetAttribute<String>("snd")));
            m_commonsounds = GetSubSystem<Audio.SoundSystem>().CreateManager(BuildPath(basepath, filesection.GetAttribute<String>("common.snd")));
            m_fightsprites = GetSubSystem<Drawing.SpriteSystem>().CreateManager(BuildPath(basepath, filesection.GetAttribute<String>("sff")), false);
            m_fxsprites = GetSubSystem<Drawing.SpriteSystem>().CreateManager(BuildPath(basepath, filesection.GetAttribute<String>("fightfx.sff")), false);
            m_fightanimations = GetSubSystem<Animations.AnimationSystem>().CreateManager(textfile.Filepath);
            m_fxanimations = GetSubSystem<Animations.AnimationSystem>().CreateManager(BuildPath(basepath, filesection.GetAttribute<String>("fightfx.air")));
            m_elements = new Elements.Collection(FightSprites, FightAnimations, FightSounds, Fonts);
            m_roundinfo = new RoundInformation(this, textfile);
            m_team1 = new Team(this, TeamSide.Left);
            m_team2 = new Team(this, TeamSide.Right);
            m_combatcheck = new CombatChecker(this);
            m_logic = new Logic.PreIntro(this);
			m_clock = new Clock(this);
        }

        Drawing.FontMap BuildFontMap(IO.TextSection filesection)
        {
            if (filesection == null) throw new ArgumentNullException("filesection");

            Dictionary<Int32, Drawing.Font> fonts = new Dictionary<Int32, Drawing.Font>();

            String fontpath1 = filesection.GetAttribute<String>("font1", null);
            if (fontpath1 != null) fonts[1] = GetSubSystem<Drawing.SpriteSystem>().LoadFont(fontpath1);

            String fontpath2 = filesection.GetAttribute<String>("font2", null);
            if (fontpath2 != null) fonts[2] = GetSubSystem<Drawing.SpriteSystem>().LoadFont(fontpath2);

            String fontpath3 = filesection.GetAttribute<String>("font3", null);
            if (fontpath3 != null) fonts[3] = GetSubSystem<Drawing.SpriteSystem>().LoadFont(fontpath3);

            Drawing.FontMap fontmap = new Drawing.FontMap(fonts);
            return fontmap;
        }

        String BuildPath(String basepath, String filepath)
        {
            IO.FileSystem filesystem = GetSubSystem<IO.FileSystem>();

            String path1 = filesystem.CombinePaths(basepath, filepath);
            if (filesystem.DoesFileExist(path1) == true) return path1;

            String path2 = filesystem.CombinePaths("data", filepath);
            if (filesystem.DoesFileExist(path1) == true) return path2;

            String path3 = filesystem.CombinePaths("", filepath);
            if (filesystem.DoesFileExist(path1) == true) return path3;

            return null;
        }

        public void Reset()
        {
            m_logic = new Logic.PreIntro(this);
            m_slowspeedbuffer = 0;
            m_tickcount = 0;
            Speed = GameSpeed.Normal;

            RoundNumber = 1;

            FightSounds.Stop();
            CommonSounds.Stop();

            Stage.Reset();
            m_stage.Reset();
            Camera.Reset();
            Pause.Reset();
            SuperPause.Reset();
            Assertions.Reset();
            EnvironmentColor.Reset();
            EnvironmentShake.Reset();
            //Team1.Clear();
            //Team2.Clear();

            Elements.Reset();
        }

        protected override void Dispose(Boolean disposing)
        {
            if (disposing == true)
            {
            }

            base.Dispose(disposing);
        }

        public Team GetWinningTeam()
        {
            if (Team1.VictoryStatus.Win == true) return Team1;
            else if (Team2.VictoryStatus.Win == true) return Team2;
            else return null;
        }

        public void Set(EngineInitialization initialization)
        {
            if (initialization == null) throw new ArgumentNullException("initialization");

            m_stage = new Stage(this, initialization.Stage);

            Team1.CreatePlayers(initialization.P1, null);
            Team2.CreatePlayers(initialization.P2, null);

            Reset();
        }

        void Tester(Boolean pressed)
        {
#if DEBUG
            if (pressed)
            {
                Team2.MainPlayer.Life = 1;
                //Team1.MainPlayer.StateManager.ChangeState(1200);
            }
#endif
        }

        void PlayerRestore(Player player)
        {
            if (player == null) throw new ArgumentNullException("player");

            player.Life = player.Constants.MaximumLife;
            player.Power = player.Constants.MaximumPower;
        }

        void RestoreLifeAndPower(Boolean pressed)
        {
            if (pressed == true)
            {
                if (RoundState == RoundState.Fight)
                {
                    Clock.Time = GetSubSystem<InitializationSettings>().RoundLength;

                    Team1.DoAction(PlayerRestore);
                    Team2.DoAction(PlayerRestore);
                }
            }
        }

        public Int32 GenerateCharacterId()
        {
            return m_idcounter++;
        }

        public void SetInput(Input.InputState inputstate)
        {
            if (inputstate == null) throw new ArgumentNullException("inputstate");

            foreach (PlayerButton button in Enum.GetValues(typeof(PlayerButton)))
            {
                PlayerButton b = button;

                inputstate[1].Add(b, x => Team1.MainPlayer.RecieveInput(b, x));
                inputstate[2].Add(b, x => Team2.MainPlayer.RecieveInput(b, x));
            }

            inputstate[0].Add(SystemButton.FullLifeAndPower, RestoreLifeAndPower);
            inputstate[0].Add(SystemButton.TestCheat, Tester);
        }

        Boolean SpeedSkip()
        {
            if (Speed == GameSpeed.Slow)
            {
                ++m_slowspeedbuffer;

                if (m_slowspeedbuffer < 3) return false;
            }

            m_slowspeedbuffer = 0;
            return true;
        }

        void UpdatePauses()
        {
            if (SuperPause.IsActive == true)
            {
                SuperPause.Update();
            }
            else
            {
                Pause.Update();
            }
        }

        void UpdateLogic()
        {
            m_logic.Update();

            if (m_logic.IsFinished() == true)
            {
                if (m_logic is Logic.PreIntro)
                {
                    m_logic = new Logic.ShowCharacterIntro(this);
                }
                else if (m_logic is Logic.ShowCharacterIntro)
                {
                    m_logic = new Logic.DisplayRoundNumber(this);
                }
                else if (m_logic is Logic.DisplayRoundNumber)
                {
                    m_logic = new Logic.ShowFight(this);
                }
                else if (m_logic is Logic.ShowFight)
                {
                    m_logic = new Logic.Fighting(this);
                }
                else if (m_logic is Logic.Fighting)
                {
                    m_logic = new Logic.CombatOver(this);
                }
                else if (m_logic is Logic.CombatOver)
                {
                    m_logic = new Logic.ShowWinPose(this);
                }
                else if (m_logic is Logic.ShowWinPose)
                {
                    if (Team1.Wins.Count >= RoundInformation.NumberOfRounds || Team2.Wins.Count >= RoundInformation.NumberOfRounds)
                    {
                        GetMainSystem<Menus.MenuSystem>().PostEvent(new Events.SwitchScreen(ScreenType.Title));
                        m_logic = new Logic.NoMoreFighting(this);
                    }
                    else
                    {
                        RoundNumber += 1;
                        m_logic = new Logic.PreIntro(this);
                    }
                }
            }
        }

        public void Update(GameTime time)
        {
            if (SpeedSkip() == false) return;

            UpdatePauses();

            Elements.Update();

            UpdateLogic();

            Assertions.Reset();

            Stage.PaletteFx.Update();
            if (Pause.IsPaused(Stage) == false && SuperPause.IsPaused(Stage) == false) Stage.Update(time);

            EnvironmentColor.Update();

            Entities.Update(time);
            m_combatcheck.Run();

            Team1.Display.Update();
            Team2.Display.Update();

            EnvironmentShake.Update();
            Camera.Update();

            GetSubSystem<Diagnostics.DiagnosticSystem>().Update(this);

            ++m_tickcount;
        }

        public void Draw(Boolean debug)
        {
            if (EnvironmentColor.IsActive == false && Assertions.NoBackLayer == false) Stage.Draw(BackgroundLayer.Back);

            if (EnvironmentColor.IsActive == true && EnvironmentColor.UnderFlag == true) EnvironmentColor.Draw();

            if (Assertions.NoBarDisplay == false)
            {
                Team1.Display.Draw();
                Team2.Display.Draw();
				Clock.Draw();
            }

            Entities.Draw(debug);

            if (EnvironmentColor.IsActive == true && EnvironmentColor.UnderFlag == false) EnvironmentColor.Draw();

            if (EnvironmentColor.IsActive == false && Assertions.NoFrontLayer == false) Stage.Draw(BackgroundLayer.Front);

            if (m_logic != null) m_logic.Draw();

            Team1.Display.ComboCounter.Draw();
            Team2.Display.ComboCounter.Draw();
        }

        public void Print(Drawing.PrintData printdata, Vector2 location, String text, Rectangle? scissorrect)
        {
            if (text == null) throw new ArgumentNullException("text");

            m_fontmap.Print(printdata, location, text, scissorrect);
        }

        public Boolean IsMatchOver()
        {
            return m_logic is Logic.ShowWinPose && m_logic.TickCount >= 0 && (Team1.Wins.Count >= RoundInformation.NumberOfRounds || Team2.Wins.Count >= RoundInformation.NumberOfRounds);
        }

        public Audio.SoundManager FightSounds
        {
            get { return m_fightsounds; }
        }

        public Audio.SoundManager CommonSounds
        {
            get { return m_commonsounds; }
        }

        public Drawing.SpriteManager FightSprites
        {
            get { return m_fightsprites; }
        }

        public Drawing.SpriteManager FxSprites
        {
            get { return m_fxsprites; }
        }

        public Animations.AnimationManager FightAnimations
        {
            get { return m_fightanimations; }
        }

        public Animations.AnimationManager FxAnimations
        {
            get { return m_fxanimations; }
        }

        public Drawing.FontMap Fonts
        {
            get { return m_fontmap; }
        }

        public Stage Stage
        {
            get { return m_stage; }
        }

        public RoundState RoundState
        {
            get
            {
                return (m_logic == null) ? RoundState.None : m_logic.State;
                //return m_state.State;
            }
        }

        public Int32 RoundNumber
        {
            get { return m_roundnumber; }

            set { m_roundnumber = value; }
        }

        public EntityCollection Entities
        {
            get { return m_entities; }
        }

        public Int32 TickCount
        {
            get { return m_tickcount; }
        }

        public Pause Pause
        {
            get { return m_pause; }
        }

        public Pause SuperPause
        {
            get { return m_superpause; }
        }

        public EngineAssertions Assertions
        {
            get { return m_asserts; }
        }

        public Camera Camera
        {
            get { return m_camera; }
        }

        public EnvironmentColor EnvironmentColor
        {
            get { return m_envcolor; }
        }

        public RoundInformation RoundInformation
        {
            get { return m_roundinfo; }
        }

        public Team Team1
        {
            get { return m_team1; }
        }

        public Team Team2
        {
            get { return m_team2; }
        }

        public EnvironmentShake EnvironmentShake
        {
            get { return m_envshake; }
        }

        public GameSpeed Speed
        {
            get { return m_speed; }

            set { m_speed = value; }
        }

        public Elements.Collection Elements
        {
            get { return m_elements; }
        }

		public Clock Clock
		{
			get { return m_clock; }
		}

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Audio.SoundManager m_fightsounds;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Audio.SoundManager m_commonsounds;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Drawing.SpriteManager m_fightsprites;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Drawing.SpriteManager m_fxsprites;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Animations.AnimationManager m_fightanimations;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Animations.AnimationManager m_fxanimations;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Stage m_stage;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly EntityCollection m_entities;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 m_roundnumber;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 m_idcounter;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 m_tickcount;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Pause m_pause;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Pause m_superpause;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly EngineAssertions m_asserts;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Camera m_camera;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly EnvironmentColor m_envcolor;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Drawing.FontMap m_fontmap;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly RoundInformation m_roundinfo;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Team m_team1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Team m_team2;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly EnvironmentShake m_envshake;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly CombatChecker m_combatcheck;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        GameSpeed m_speed;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Int32 m_slowspeedbuffer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        Logic.Base m_logic;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Elements.Collection m_elements;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		readonly Clock m_clock;

        #endregion
    }
}