using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

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

            Entities = new EntityCollection(this);
            Pause = new Pause(this, false);
            SuperPause = new Pause(this, true);
            Assertions = new EngineAssertions();
            Camera = new Camera(this);
            EnvironmentColor = new EnvironmentColor(this);
            EnvironmentShake = new EnvironmentShake(this);
            Speed = GameSpeed.Normal;
            Fonts = BuildFontMap(filesection);
            FightSounds = GetSubSystem<Audio.SoundSystem>().CreateManager(BuildPath(basepath, filesection.GetAttribute<string>("snd")));
            CommonSounds = GetSubSystem<Audio.SoundSystem>().CreateManager(BuildPath(basepath, filesection.GetAttribute<string>("common.snd")));
            FightSprites = GetSubSystem<Drawing.SpriteSystem>().CreateManager(BuildPath(basepath, filesection.GetAttribute<string>("sff")));
            FxSprites = GetSubSystem<Drawing.SpriteSystem>().CreateManager(BuildPath(basepath, filesection.GetAttribute<string>("fightfx.sff")));
            FightAnimations = GetSubSystem<Animations.AnimationSystem>().CreateManager(textfile.Filepath);
            FxAnimations = GetSubSystem<Animations.AnimationSystem>().CreateManager(BuildPath(basepath, filesection.GetAttribute<string>("fightfx.air")));
            Elements = new Elements.Collection(FightSprites, FightAnimations, FightSounds, Fonts);
            RoundInformation = new RoundInformation(this, textfile);
            Team1 = new Team(this, TeamSide.Left);
            Team2 = new Team(this, TeamSide.Right);
            m_combatcheck = new CombatChecker(this);
            m_logic = new Logic.PreIntro(this);
			Clock = new Clock(this);
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
            TickCount = 0;
            Speed = GameSpeed.Normal;

            MatchNumber = 1;
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
            Elements.Reset();
        }

        public void Set(EngineInitialization init)
        {
			if (init == null) throw new ArgumentNullException(nameof(init));

			Initialization = init;

            Stage = new Stage(this, init.Stage);

            Team1.CreatePlayers(init.Team1Mode, init.Team1P1, init.Team1P2);
            Team2.CreatePlayers(init.Team2Mode, init.Team2P1, init.Team2P2);

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
                    return;
                }

                if (m_logic is Logic.ShowCharacterIntro)
                {
                    m_logic = new Logic.DisplayRoundNumber(this);
                    return;
                }

                if (m_logic is Logic.DisplayRoundNumber)
                {
                    m_logic = new Logic.ShowFight(this);
                    return;
                }

                if (m_logic is Logic.ShowFight)
                {
                    m_logic = new Logic.Fighting(this);
                    return;
                }

                if (m_logic is Logic.Fighting)
                {
                    m_logic = new Logic.CombatOver(this);
                    return;
                }

                if (m_logic is Logic.CombatOver)
                {
                    m_logic = new Logic.ShowWinPose(this);
                    return;
                }

                if (!(m_logic is Logic.ShowWinPose)) return;
                
                if (Team1.Wins.Count >= RoundInformation.NumberOfRounds || Team2.Wins.Count >= RoundInformation.NumberOfRounds)
                {
                    if (Initialization.Mode == CombatMode.Arcade)
                    {
                        var index = Team2.MainPlayer.BasePlayer.Profile.ProfileLoader.PlayerProfiles
                                        .Select(o => o.Profile).ToList()
                                        .IndexOf(Team2.MainPlayer.BasePlayer.Profile) + 1;
                        if (index == Team2.MainPlayer.BasePlayer.Profile.ProfileLoader.PlayerProfiles.Count)
                        {
                            GetMainSystem<Menus.MenuSystem>().PostEvent(new Events.SwitchScreen(ScreenType.Title));
                            m_logic = new Logic.NoMoreFighting(this);
                            return;
                        }
                            
                        RoundNumber = 1;
                        MatchNumber++;
                        
                        // same team 1
                        Team1.Clear();
                        Team1.CreatePlayers(Initialization.Team1Mode, Initialization.Team1P1, Initialization.Team1P2);

                        // update team 2
                        var profile = Team2.MainPlayer.BasePlayer.Profile.ProfileLoader.PlayerProfiles[index].Profile;
                        Team2.Clear();
                        Team2.CreatePlayers(Initialization.Team2Mode, 
                            new PlayerCreation(profile, 0, PlayerMode.Ai),
                            null);
                        m_logic = new Logic.PreIntro(this);
                        return;
                    }

                    GetMainSystem<Menus.MenuSystem>().PostEvent(new Events.SwitchScreen(ScreenType.Title));
                    m_logic = new Logic.NoMoreFighting(this);
                    return;
                }

                RoundNumber++;
                m_logic = new Logic.PreIntro(this);
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

            ++TickCount;
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

            m_logic?.Draw();

            Team1.Display.ComboCounter.Draw();
            Team2.Display.ComboCounter.Draw();
        }

        public void Print(Drawing.PrintData printdata, Vector2 location, string text, Rectangle? scissorrect)
        {
            if (text == null) throw new ArgumentNullException(nameof(text));

            Fonts.Print(printdata, location, text, scissorrect);
        }

        public bool IsMatchOver()
        {
            return m_logic is Logic.ShowWinPose && m_logic.TickCount >= 0 && (Team1.Wins.Count >= RoundInformation.NumberOfRounds || Team2.Wins.Count >= RoundInformation.NumberOfRounds);
        }

        private Audio.SoundManager FightSounds { get; }

        public Audio.SoundManager CommonSounds { get; }

        private Drawing.SpriteManager FightSprites { get; }

        public Drawing.SpriteManager FxSprites { get; }

        private Animations.AnimationManager FightAnimations { get; }

        public Animations.AnimationManager FxAnimations { get; }

        public Drawing.FontMap Fonts { get; }

        public Stage Stage { get; private set; }

        public RoundState RoundState => m_logic == null ? RoundState.None : m_logic.State;
        
        public int MatchNumber { get; private set; }

        public int RoundNumber { get; private set; }

        public EntityCollection Entities { get; }

        public int TickCount { get; private set; }

        public Pause Pause { get; }

        public Pause SuperPause { get; }

        public EngineAssertions Assertions { get; }

        public Camera Camera { get; }

        public EnvironmentColor EnvironmentColor { get; }

        public RoundInformation RoundInformation { get; }

        public Team Team1 { get; }

        public Team Team2 { get; }

        public EnvironmentShake EnvironmentShake { get; }

        public GameSpeed Speed { get; set; }

        public Elements.Collection Elements { get; }

        public Clock Clock { get; }

        public EngineInitialization Initialization { get; private set; }

        #region Fields

        private int m_idcounter;
        private readonly CombatChecker m_combatcheck;
        private int m_slowspeedbuffer;
        private Logic.Base m_logic;

        #endregion
    }
}