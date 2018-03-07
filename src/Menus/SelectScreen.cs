using System;
using System.Diagnostics;
using xnaMugen.IO;
using xnaMugen.Drawing;
using xnaMugen.Collections;
using Microsoft.Xna.Framework;
using System.Text;
using xnaMugen.Elements;
using Microsoft.Xna.Framework.Graphics;

namespace xnaMugen.Menus
{
    internal class SelectScreen : NonCombatScreen
    {
        public SelectScreen(MenuSystem screensystem, TextSection textsection, string spritepath, string animationpath, string soundpath)
            : base(screensystem, textsection, spritepath, animationpath, soundpath)
        {
            m_elements = new Collection(SpriteManager, AnimationManager, SoundManager, MenuSystem.FontMap);
            Grid = new SelectGrid(textsection, m_elements, PlayerProfiles);
            m_elements.Build(textsection, "cell.random");

            m_cursorblinking = textsection.GetAttribute<bool>("p2.cursor.blink");
            m_soundcancel = textsection.GetAttribute<SoundId>("cancel.snd");
            m_titlelocation = textsection.GetAttribute<Point>("title.offset");
            m_titlefont = textsection.GetAttribute<PrintData>("title.font");
            m_stageposition = textsection.GetAttribute<Point>("stage.pos");
            m_soundstagemove = textsection.GetAttribute<SoundId>("stage.move.snd");
            m_soundstageselect = textsection.GetAttribute<SoundId>("stage.done.snd");
            m_stagefont1 = textsection.GetAttribute<PrintData>("stage.active.font");
            m_stagefont2 = textsection.GetAttribute<PrintData>("stage.active2.font");
            m_stagedonefont = textsection.GetAttribute<PrintData>("stage.done.font");
            m_randomswitchtime = textsection.GetAttribute("cell.random.switchtime", 5);
            m_p1info = new SelectData(this, MenuSystem.GetSubSystem<Input.InputSystem>().CurrentInput[1], textsection, "p1", Grid.MoveOverEmptyBoxes);
            m_p2info = new SelectData(this, MenuSystem.GetSubSystem<Input.InputSystem>().CurrentInput[2], textsection, "p2", Grid.MoveOverEmptyBoxes);
            m_p1TeamInfo = new TeamSelectData(this, MenuSystem.GetSubSystem<Input.InputSystem>().CurrentInput[1], textsection, "p1", Grid.MoveOverEmptyBoxes);
            m_p2TeamInfo = new TeamSelectData(this, MenuSystem.GetSubSystem<Input.InputSystem>().CurrentInput[2], textsection, "p2", Grid.MoveOverEmptyBoxes);
            m_isdone = false;
            m_stagedisplaybuilder = new StringBuilder();
        }

        public override void SetInput(Input.InputState inputstate)
        {
            base.SetInput(inputstate);

            inputstate[0].Add(SystemButton.Quit, BackToTitleScreen);

            switch (CombatMode)
            {
                case CombatMode.Versus:
                    SetCharacterSelectionInput(m_p1info);
                    SetCharacterSelectionInput(m_p2info);
                    break;
                case CombatMode.TeamVersus:
                    SetTeamModeCharacterSelectionInput(m_p1TeamInfo);
                    SetTeamModeCharacterSelectionInput(m_p2TeamInfo);
                    break;
            }
        }

        public override void Reset()
        {
            base.Reset();

            m_blinkval = 0;
            m_p1info.Reset();
            m_p2info.Reset();
            m_p1TeamInfo.Reset();
            m_p2TeamInfo.Reset();

            m_stageselected = false;
            m_stageselector = null;
            m_currentstage = -1;

            m_isdone = false;

            m_elements.Reset();
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);

            if (++m_blinkval > 6) m_blinkval = -6;

            m_p1info.Update();
            m_p2info.Update();

            m_elements.Update();

            m_p1TeamInfo.Update();
            m_p2TeamInfo.Update();

            CheckReady();
        }

        public override void Draw(bool debugdraw)
        {
            base.Draw(debugdraw);

            if (CombatMode == CombatMode.Versus)
            {
                Grid.Draw();
                DrawCursorGrid();
                DrawFace(m_p1info);
                DrawFace(m_p2info);
            }
            else if (CombatMode == CombatMode.TeamVersus)
            {
                Grid.Draw();
                DrawCursorGrid();
                if (m_p1TeamInfo.State == TeamSelectState.SelectSelf)
                {
                    DrawFace(m_p1TeamInfo.P1SelectData);
                }
                else if (m_p1TeamInfo.State == TeamSelectState.SelectMate)
                {
                    DrawFace(m_p1TeamInfo.P2SelectData);
                }
                if (m_p2TeamInfo.State == TeamSelectState.SelectSelf)
                {
                    DrawFace(m_p2TeamInfo.P1SelectData);
                }
                else if (m_p2TeamInfo.State == TeamSelectState.SelectMate)
                {
                    DrawFace(m_p2TeamInfo.P2SelectData);
                }
            }
            DrawStage();
            Print(m_titlefont, (Vector2)m_titlelocation, VersusMode, null);

            if (CombatMode == CombatMode.TeamVersus)
            {
                m_p1TeamInfo.Draw();
                m_p2TeamInfo.Draw();
            }
        }

        private void DrawCursorGrid()
        {
            for (var y = 0; y != Grid.Size.Y; ++y)
            {
                for (var x = 0; x != Grid.Size.X; ++x)
                {
                    var xy = new Point(x, y);

                    var location = (Vector2)Grid.GridPosition;
                    location.X += (Grid.CellSize.X + Grid.CellSpacing) * x;
                    location.Y += (Grid.CellSize.Y + Grid.CellSpacing) * y;

                    var selection = Grid.GetSelection(xy, false);
                    if (selection != null && selection.SelectionType == PlayerSelectType.Profile) 
                        selection.Profile.SpriteManager.Draw(SpriteId.SmallPortrait, location, Vector2.Zero, Vector2.One, SpriteEffects.None);

                    if (selection != null && selection.SelectionType == PlayerSelectType.Random)
                    {
                        var randomimage = m_elements.GetElement("cell.random") as StaticImage;
                        if (randomimage != null) randomimage.Draw(location);
                    }

                    if (CombatMode == CombatMode.Versus)
                    {
                        if (m_p1info.CurrentCell == xy && m_p2info.CurrentCell == xy)
                        {
                            if (m_blinkval > 0) m_p1info.DrawCursorActive(location);
                            else m_p2info.DrawCursorActive(location);
                        }
                        else if (m_p1info.CurrentCell == xy)
                        {
                            m_p1info.DrawCursorActive(location);
                        }
                        else if (m_p2info.CurrentCell == xy)
                        {
                            m_p2info.DrawCursorActive(location);
                        }
                    }
                    else if (CombatMode == CombatMode.TeamVersus)
                    {
                        var p1 = GetSelectData(m_p1TeamInfo);
                        var p2 = GetSelectData(m_p2TeamInfo);;
                        if (p1?.CurrentCell == xy && p2?.CurrentCell == xy)
                        {
                            if (m_blinkval > 0) p1?.DrawCursorActive(location);
                            else p2?.DrawCursorActive(location);
                        }
                        else if (p1?.CurrentCell == xy)
                        {
                            p1?.DrawCursorActive(location);
                        }
                        else if (p2?.CurrentCell == xy)
                        {
                            p2?.DrawCursorActive(location);
                        }
                    }
                }
            }
        }

        private static SelectData GetSelectData(TeamSelectData data)
        {
            if (data.State == TeamSelectState.SelectSelf)
            {
                return data.P1SelectData;
            }
            if (data.State == TeamSelectState.SelectMate)
            {
                return data.P2SelectData;
            }
            return null;
        }

        private void DrawStage()
        {
            if (m_stageselector == null) return;

            var pd = m_blinkval > 0 ? m_stagefont1 : m_stagefont2;
            if (m_stageselected) pd = m_stagedonefont;

            var sp = m_currentstage >= 0 && m_currentstage < StageProfiles.Count ? StageProfiles[m_currentstage] : null;

            m_stagedisplaybuilder.Length = 0;
            if (sp != null)
            {
                m_stagedisplaybuilder.AppendFormat("Stage {0}: {1}", m_currentstage + 1, sp.Name);
            }
            else
            {
                m_stagedisplaybuilder.Append("Stage: Random");
            }

            Print(pd, (Vector2)m_stageposition, m_stagedisplaybuilder.ToString(), null);
        }

        private void DrawFace(SelectData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var selection = Grid.GetSelection(data.CurrentCell, false);
            if (selection == null) return;

            if (selection.SelectionType == PlayerSelectType.Profile)
            {
                data.DrawProfile(selection.Profile);
            }
        }

        private void SetCharacterSelectionInput(TeamSelectData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            data.ButtonMap.Clear();
            data.ButtonMap.Add(PlayerButton.Up, x => { if (x) MoveCharacterSelection(data, CursorDirection.Up); });
            data.ButtonMap.Add(PlayerButton.Down, x => { if (x) MoveCharacterSelection(data, CursorDirection.Down); });
            data.ButtonMap.Add(PlayerButton.Left, x => { if (x) MoveCharacterSelection(data, CursorDirection.Left); });
            data.ButtonMap.Add(PlayerButton.Right, x => { if (x) MoveCharacterSelection(data, CursorDirection.Right); });
            data.ButtonMap.Add(PlayerButton.A, x => { if (x) SelectCharacter(data, 0); });
            data.ButtonMap.Add(PlayerButton.B, x => { if (x) SelectCharacter(data, 1); });
            data.ButtonMap.Add(PlayerButton.C, x => { if (x) SelectCharacter(data, 2); });
            data.ButtonMap.Add(PlayerButton.X, x => { if (x) SelectCharacter(data, 3); });
            data.ButtonMap.Add(PlayerButton.Y, x => { if (x) SelectCharacter(data, 4); });
            data.ButtonMap.Add(PlayerButton.Z, x => { if (x) SelectCharacter(data, 5); });
        }

        private void SetCharacterSelectionInput(SelectData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            data.ButtonMap.Clear();
            data.ButtonMap.Add(PlayerButton.Up, x => { if (x) MoveCharacterSelection(data, CursorDirection.Up); });
            data.ButtonMap.Add(PlayerButton.Down, x => { if (x) MoveCharacterSelection(data, CursorDirection.Down); });
            data.ButtonMap.Add(PlayerButton.Left, x => { if (x) MoveCharacterSelection(data, CursorDirection.Left); });
            data.ButtonMap.Add(PlayerButton.Right, x => { if (x) MoveCharacterSelection(data, CursorDirection.Right); });
            data.ButtonMap.Add(PlayerButton.Taunt, x => { CharacterPalletteShift(data, x); });
            data.ButtonMap.Add(PlayerButton.A, x => { if (x) SelectCharacter(data, 0); });
            data.ButtonMap.Add(PlayerButton.B, x => { if (x) SelectCharacter(data, 1); });
            data.ButtonMap.Add(PlayerButton.C, x => { if (x) SelectCharacter(data, 2); });
            data.ButtonMap.Add(PlayerButton.X, x => { if (x) SelectCharacter(data, 3); });
            data.ButtonMap.Add(PlayerButton.Y, x => { if (x) SelectCharacter(data, 4); });
            data.ButtonMap.Add(PlayerButton.Z, x => { if (x) SelectCharacter(data, 5); });
        }

        private void SetTeamModeCharacterSelectionInput(TeamSelectData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            data.ButtonMap.Clear();
            data.ButtonMap.Add(PlayerButton.Up, x => { if (x) MoveTeamModeSelection(data, CursorDirection.Up); });
            data.ButtonMap.Add(PlayerButton.Down, x => { if (x) MoveTeamModeSelection(data, CursorDirection.Down); });
            data.ButtonMap.Add(PlayerButton.A, x => { if (x) SelectTeamMode(data); });
            data.ButtonMap.Add(PlayerButton.B, x => { if (x) SelectTeamMode(data); });
            data.ButtonMap.Add(PlayerButton.C, x => { if (x) SelectTeamMode(data); });
            data.ButtonMap.Add(PlayerButton.X, x => { if (x) SelectTeamMode(data); });
            data.ButtonMap.Add(PlayerButton.Y, x => { if (x) SelectTeamMode(data); });
            data.ButtonMap.Add(PlayerButton.Z, x => { if (x) SelectTeamMode(data); });
        }

        private void CharacterPalletteShift(SelectData data, bool pressed)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (pressed)
            {
                data.PaletteIndex += 6;
            }
            else
            {
                data.PaletteIndex -= 6;
            }
        }

        private void MoveCharacterSelection(TeamSelectData data, CursorDirection direction)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (data.MoveCharacterCursor(direction, Grid.Size, Grid.Wrapping))
            {
                data.PlayCursorMoveSound();
            }
        }

        private void MoveCharacterSelection(SelectData data, CursorDirection direction)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (data.MoveCharacterCursor(direction, Grid.Size, Grid.Wrapping))
            {
                data.PlayCursorMoveSound();
            }
        }

        private void MoveTeamModeSelection(TeamSelectData data, CursorDirection direction)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (data.MoveTeamModeCursor(direction))
            {
                data.PlayCursorMoveSound();
            }
        }

        private void SelectCharacter(TeamSelectData data, int index)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var selectData = GetSelectData(data);
            var selection = Grid.GetSelection(selectData.CurrentCell, false);
            if (selection == null || selection.SelectionType != PlayerSelectType.Profile) return;

            data.PlaySelectSound();
            selectData.IsSelected = true;
            selectData.PaletteIndex += index;

            if (data.TeamMode == TeamMode.Single)
            {
                data.ButtonMap.Clear();
                SetStageSelectionInput(data.P1SelectData);
            }
            else if (data.State == TeamSelectState.SelectSelf)
            {
                data.State = TeamSelectState.SelectMate;
            }
            else
            {
                data.ButtonMap.Clear();
                SetStageSelectionInput(data.P2SelectData);
            }
        }

        private void SelectCharacter(SelectData data, int index)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var selection = Grid.GetSelection(data.CurrentCell, false);
            if (selection == null || selection.SelectionType != PlayerSelectType.Profile) return;

            data.PlaySelectSound();
            data.ButtonMap.Clear();
            data.IsSelected = true;
            data.PaletteIndex += index;

            SetStageSelectionInput(data);
        }

        private void SelectTeamMode(TeamSelectData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (data.State == TeamSelectState.TeamMode)
            {
                data.State = TeamSelectState.SelectSelf;
                data.PlaySelectSound();

                data.ButtonMap.Clear();
                SetCharacterSelectionInput(data);
            }
            else
            {
                data.State = TeamSelectState.SelectMate;
                data.PlaySelectSound();

                data.ButtonMap.Clear();
                SetCharacterSelectionInput(data);
            }
        }

        private void SetStageSelectionInput(SelectData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (m_stageselector != null) return;

            data.ButtonMap.Clear();

            data.ButtonMap.Add(PlayerButton.Left, delegate (bool pressed) { if (pressed) { MoveStageSelection(-1); } });
            data.ButtonMap.Add(PlayerButton.Right, delegate (bool pressed) { if (pressed) { MoveStageSelection(+1); } });
            data.ButtonMap.Add(PlayerButton.A, SelectCurrentStage);
            data.ButtonMap.Add(PlayerButton.B, SelectCurrentStage);
            data.ButtonMap.Add(PlayerButton.C, SelectCurrentStage);
            data.ButtonMap.Add(PlayerButton.X, SelectCurrentStage);
            data.ButtonMap.Add(PlayerButton.Y, SelectCurrentStage);
            data.ButtonMap.Add(PlayerButton.Z, SelectCurrentStage);

            m_stageselector = data;
            m_currentstage = 0;
        }

        private void MoveStageSelection(int offset)
        {
            if (offset == 0) return;
            SoundManager.Play(m_soundstagemove);

            offset = offset % StageProfiles.Count;

            if (offset > 0)
            {
                m_currentstage += offset;
                if (m_currentstage >= StageProfiles.Count)
                {
                    var diff = m_currentstage - StageProfiles.Count;
                    m_currentstage = -1 + diff;
                }
            }

            if (offset < 0)
            {
                m_currentstage += offset;
                if (m_currentstage < -1)
                {
                    var diff = m_currentstage + 2;
                    m_currentstage = StageProfiles.Count - 1 + diff;
                }
            }
        }

        private void SelectCurrentStage(bool pressed)
        {
            if (pressed)
            {
                m_stageselected = true;
                SoundManager.Play(m_soundstageselect);

                m_stageselector.ButtonMap.Clear();
            }
        }

        private void CheckReady()
        {
            if (CombatMode == CombatMode.Versus)
            {
                if (m_isdone) return;
                if (m_stageselected == false || m_p1info.IsSelected == false || m_p2info.IsSelected == false) return;

                m_isdone = true;

                if (m_currentstage == -1) m_currentstage = MenuSystem.GetSubSystem<Random>().NewInt(0, StageProfiles.Count - 1);

                var p1index = m_p1info.CurrentCell.Y * Grid.Size.X + m_p1info.CurrentCell.X;
                var p2index = m_p2info.CurrentCell.Y * Grid.Size.X + m_p2info.CurrentCell.X;

                var p1 = PlayerProfiles[p1index];
                var p2 = PlayerProfiles[p2index];
                var stage = StageProfiles[m_currentstage];

                var init = new Combat.EngineInitialization(CombatMode,
                                                           p1.Profile, m_p1info.PaletteIndex, 
                                                           p2.Profile, m_p2info.PaletteIndex, 
                                                           stage);

                MenuSystem.PostEvent(new Events.SetupCombat(init));
                MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Versus));
            }
            else if (CombatMode == CombatMode.TeamVersus)
            {
                if (m_isdone) return;
                if (m_stageselected == false || m_p1TeamInfo.IsSelected == false || m_p2TeamInfo.IsSelected == false) return;

                m_isdone = true;

                if (m_currentstage == -1) m_currentstage = MenuSystem.GetSubSystem<Random>().NewInt(0, StageProfiles.Count - 1);

                var p11index = m_p1TeamInfo.P1SelectData.CurrentCell.Y * Grid.Size.X + m_p1TeamInfo.P1SelectData.CurrentCell.X;
                var p12index = m_p1TeamInfo.P2SelectData.CurrentCell.Y * Grid.Size.X + m_p1TeamInfo.P2SelectData.CurrentCell.X;
                var p21index = m_p2TeamInfo.P1SelectData.CurrentCell.Y * Grid.Size.X + m_p2TeamInfo.P1SelectData.CurrentCell.X;
                var p22index = m_p2TeamInfo.P2SelectData.CurrentCell.Y * Grid.Size.X + m_p2TeamInfo.P2SelectData.CurrentCell.X;

                var p11 = PlayerProfiles[p11index];
                var p12 = m_p1TeamInfo.TeamMode == TeamMode.Single ? null : PlayerProfiles[p12index];
                var p21 = PlayerProfiles[p21index];
                var p22 = m_p2TeamInfo.TeamMode == TeamMode.Single ? null : PlayerProfiles[p22index];
                var stage = StageProfiles[m_currentstage];

                var init = new Combat.EngineInitialization(CombatMode, 
                                                           m_p1TeamInfo.TeamMode,
                                                           m_p2TeamInfo.TeamMode,
                                                           p11.Profile, m_p1TeamInfo.P1SelectData.PaletteIndex, 
                                                           p12?.Profile, m_p1TeamInfo.P2SelectData.PaletteIndex, 
                                                           p21.Profile, m_p2TeamInfo.P1SelectData.PaletteIndex, 
                                                           p22?.Profile, m_p2TeamInfo.P2SelectData.PaletteIndex, 
                                                           stage);

                MenuSystem.PostEvent(new Events.SetupCombat(init));
                MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Versus));
            }
        }

        private void BackToTitleScreen(bool pressed)
        {
            if (pressed)
            {
                SoundManager.Play(m_soundcancel);

                MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Title));
            }
        }

        public ListIterator<StageProfile> StageProfiles => MenuSystem.GetSubSystem<ProfileLoader>().StageProfiles;

        public ListIterator<PlayerSelect> PlayerProfiles => MenuSystem.GetSubSystem<ProfileLoader>().PlayerProfiles;

        public string VersusMode
        {
            get; private set;
        }

        public CombatMode CombatMode
        {
            get { return m_combatMode; }
            set
            {
                m_combatMode = value;
                switch (m_combatMode)
                {
                    case CombatMode.Versus:
                        VersusMode = "Versus Mode";
                        break;
                    case CombatMode.TeamVersus:
                        VersusMode = "Team Versus";
                        break;
                }
            }
        }

        public SelectGrid Grid { get; }

		#region Fields

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly bool m_cursorblinking;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_blinkval;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SoundId m_soundcancel;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SoundId m_soundstagemove;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SoundId m_soundstageselect;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_titlelocation;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PrintData m_titlefont;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly int m_randomswitchtime;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SelectData m_p1info;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly SelectData m_p2info;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Point m_stageposition;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PrintData m_stagefont1;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PrintData m_stagefont2;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly PrintData m_stagedonefont;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly Elements.Collection m_elements;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private readonly StringBuilder m_stagedisplaybuilder;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private int m_currentstage;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_stageselected;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private SelectData m_stageselector;

		[DebuggerBrowsable(DebuggerBrowsableState.Never)]
		private bool m_isdone;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CombatMode m_combatMode;
        private TeamSelectData m_p1TeamInfo;
        private TeamSelectData m_p2TeamInfo;

        #endregion
    }
}