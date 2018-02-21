using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace xnaMugen.Combat
{
    internal class FightEngine : MainSystem
    {
        public FightEngine(SubSystems subsystems)
            : base(subsystems)
        {
            var textfile = GetSubSystem<IO.FileSystem>().OpenTextFile(@"data/fight.def");
            var filesection = textfile.GetSection("Files");
            var basepath = GetSubSystem<IO.FileSystem>().GetDirectory(textfile.Filepath);

			m_init = null;
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
            m_fightsounds = GetSubSystem<Audio.SoundSystem>().CreateManager(BuildPath(basepath, filesection.GetAttribute<string>("snd")));
            m_commonsounds = GetSubSystem<Audio.SoundSystem>().CreateManager(BuildPath(basepath, filesection.GetAttribute<string>("common.snd")));
            m_fightsprites = GetSubSystem<Drawing.SpriteSystem>().CreateManager(BuildPath(basepath, filesection.GetAttribute<string>("sff")));
            m_fxsprites = GetSubSystem<Drawing.SpriteSystem>().CreateManager(BuildPath(basepath, filesection.GetAttribute<string>("fightfx.sff")));
            m_fightanimations = GetSubSystem<Animations.AnimationSystem>().CreateManager(textfile.Filepath);
            m_fxanimations = GetSubSystem<Animations.AnimationSystem>().CreateManager(BuildPath(basepath, filesection.GetAttribute<string>("fightfx.air")));
            m_elements = new Elements.Collection(FightSprites, FightAnimations, FightSounds, Fonts);
            m_roundinfo = new RoundInformation(this, textfile);
            m_team1 = new Team(this, TeamSide.Left);
            m_team2 = new Team(this, TeamSide.Right);
            m_combatcheck = new CombatChecker(this);
            m_logic = new Logic.PreIntro(this);
			m_clock = new Clock(this);
        }

        private Drawing.FontMap BuildFontMap(IO.TextSection filesection)
        {
            if (filesection == null) throw new ArgumentNullException(nameof(filesection));

            var fonts = new Dictionary<int, Drawing.Font>();

            var fontpath1 = filesection.GetAttribute<string>("font1", null);
            if (fontpath1 != null) fonts[1] = GetSubSystem<Drawing.SpriteSystem>().LoadFont(fontpath1);

            var fontpath2 = filesection.GetAttribute<string>("font2", null);
            if (fontpath2 != null) fonts[2] = GetSubSystem<Drawing.SpriteSystem>().LoadFont(fontpath2);

            var fontpath3 = filesection.GetAttribute<string>("font3", null);
            if (fontpath3 != null) fonts[3] = GetSubSystem<Drawing.SpriteSystem>().LoadFont(fontpath3);

            var fontmap = new Drawing.FontMap(fonts);
            return fontmap;
        }

        private string BuildPath(string basepath, string filepath)
        {
            var filesystem = GetSubSystem<IO.FileSystem>();

            var path1 = filesystem.CombinePaths(basepath, filepath);
            if (filesystem.DoesFileExist(path1)) return path1;

            var path2 = filesystem.CombinePaths("data", filepath);
            if (filesystem.DoesFileExist(path1)) return path2;

            var path3 = filesystem.CombinePaths("", filepath);
            if (filesystem.DoesFileExist(path1)) return path3;

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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
            }

            base.Dispose(disposing);
        }

        public Team GetWinningTeam()
        {
            if (Team1.VictoryStatus.Win) return Team1;
            if (Team2.VictoryStatus.Win) return Team2;
            return null;
        }

        public void Set(EngineInitialization init)
        {
			if (init == null) throw new ArgumentNullException(nameof(init));

			m_init = init;

            m_stage = new Stage(this, init.Stage);

            Team1.CreatePlayers(init.P1, null);
            Team2.CreatePlayers(init.P2, null);

			GetSubSystem<Random>().Seed(init.Seed);

            Reset();
        }

        private void Tester(bool pressed)
        {
#if DEBUG
            if (pressed)
            {
                Team2.MainPlayer.Life = 1;
                //Team1.MainPlayer.StateManager.ChangeState(1200);
            }
#endif
        }

        private void PlayerRestore(Player player)
        {
            if (player == null) throw new ArgumentNullException(nameof(player));

            player.Life = player.Constants.MaximumLife;
            player.Power = player.Constants.MaximumPower;
        }

        private void RestoreLifeAndPower(bool pressed)
        {
            if (pressed)
            {
                if (RoundState == RoundState.Fight)
                {
                    Clock.Time = GetSubSystem<InitializationSettings>().RoundLength;

                    Team1.DoAction(PlayerRestore);
                    Team2.DoAction(PlayerRestore);
                }
            }
        }

        public int GenerateCharacterId()
        {
            return m_idcounter++;
        }

        public void SetInput(Input.InputState inputstate)
        {
            if (inputstate == null) throw new ArgumentNullException(nameof(inputstate));

            foreach (PlayerButton button in Enum.GetValues(typeof(PlayerButton)))
            {
                var b = button;

                inputstate[1].Add(b, x => Team1.MainPlayer.RecieveInput(b, x));
                inputstate[2].Add(b, x => Team2.MainPlayer.RecieveInput(b, x));
            }

            inputstate[0].Add(SystemButton.FullLifeAndPower, RestoreLifeAndPower);
            inputstate[0].Add(SystemButton.TestCheat, Tester);
        }

        private bool SpeedSkip()
        {
            if (Speed == GameSpeed.Slow)
            {
                ++m_slowspeedbuffer;

                if (m_slowspeedbuffer < 3) return false;
            }

            m_slowspeedbuffer = 0;
            return true;
        }

        private void UpdatePauses()
        {
            if (SuperPause.IsActive)
            {
                SuperPause.Update();
            }
            else
            {
                Pause.Update();
            }
        }

        private void UpdateLogic()
        {
            m_logic.Update();

            if (m_logic.IsFinished())
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

        public void Draw(bool debug)
        {
            if (EnvironmentColor.IsActive == false && Assertions.NoBackLayer == false) Stage.Draw(BackgroundLayer.Back);

            if (EnvironmentColor.IsActive && EnvironmentColor.UnderFlag) EnvironmentColor.Draw();

            if (Assertions.NoBarDisplay == false)
            {
                Team1.Display.Draw();
                Team2.Display.Draw();
				Clock.Draw();
            }

            Entities.Draw(debug);

            if (EnvironmentColor.IsActive && EnvironmentColor.UnderFlag == false) EnvironmentColor.Draw();

            if (EnvironmentColor.IsActive == false && Assertions.NoFrontLayer == false) Stage.Draw(BackgroundLayer.Front);

            if (m_logic != null) m_logic.Draw();

            Team1.Display.ComboCounter.Draw();
            Team2.Display.ComboCounter.Draw();
        }

        public void Print(Drawing.PrintData printdata, Vector2 location, string text, Rectangle? scissorrect)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            m_fontmap.Print(printdata, location, text, scissorrect);
        }

        public bool IsMatchOver()
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

        public Drawing.SpriteManager FightSprites => m_fightsprites;

        public Drawing.SpriteManager FxSprites => m_fxsprites;

        public Animations.AnimationManager FightAnimations => m_fightanimations;

        public Animations.AnimationManager FxAnimations => m_fxanimations;

        public Drawing.FontMap Fonts => m_fontmap;

        public Stage Stage => m_stage;

        public RoundState RoundState => m_logic == null ? RoundState.None : m_logic.State;

        public int RoundNumber
        {
            get => m_roundnumber;

            set { m_roundnumber = value; }
        }

        public EntityCollection Entities => m_entities;

        public int TickCount => m_tickcount;

        public Pause Pause => m_pause;

        public Pause SuperPause => m_superpause;

        public EngineAssertions Assertions => m_asserts;

        public Camera Camera => m_camera;

        public EnvironmentColor EnvironmentColor => m_envcolor;

        public RoundInformation RoundInformation => m_roundinfo;

        public Team Team1 => m_team1;

        public Team Team2 => m_team2;

        public EnvironmentShake EnvironmentShake => m_envshake;

        public GameSpeed Speed
        {
            get => m_speed;

            set { m_speed = value; }
        }

        public Elements.Collection Elements => m_elements;

        public Clock Clock => m_clock;

        public EngineInitialization Initialization => m_init;

        #region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private EngineInitialization m_init;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Audio.SoundManager m_fightsounds;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        readonly Audio.SoundManager m_commonsounds;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Drawing.SpriteManager m_fightsprites;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Drawing.SpriteManager m_fxsprites;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Animations.AnimationManager m_fightanimations;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Animations.AnimationManager m_fxanimations;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Stage m_stage;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly EntityCollection m_entities;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_roundnumber;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_idcounter;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_tickcount;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Pause m_pause;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Pause m_superpause;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly EngineAssertions m_asserts;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Camera m_camera;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly EnvironmentColor m_envcolor;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Drawing.FontMap m_fontmap;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly RoundInformation m_roundinfo;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Team m_team1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Team m_team2;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly EnvironmentShake m_envshake;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly CombatChecker m_combatcheck;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private GameSpeed m_speed;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_slowspeedbuffer;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private Logic.Base m_logic;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Elements.Collection m_elements;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Clock m_clock;

        #endregion
    }
}