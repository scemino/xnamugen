using System;
using System.Diagnostics;
using xnaMugen.IO;
using xnaMugen.Drawing;
using xnaMugen.Collections;
using Microsoft.Xna.Framework;

namespace xnaMugen.Menus
{
    internal class TeamSelectScreenBehavior : ISelectScreenBehavior
    {
        public TeamSelectScreenBehavior(SelectScreen selectScreen, TextSection textsection)
        {
            SelectScreen = selectScreen;

            m_stageSelect = new StageSelect(selectScreen, textsection);

            m_soundcancel = textsection.GetAttribute<SoundId>("cancel.snd");
            m_titlelocation = textsection.GetAttribute<Point>("title.offset");
            m_titlefont = textsection.GetAttribute<PrintData>("title.font");

            m_p1info = new SelectData(selectScreen, selectScreen.MenuSystem.GetSubSystem<Input.InputSystem>().CurrentInput[1], textsection, "p1", Grid.MoveOverEmptyBoxes);
            m_p2info = new SelectData(selectScreen, selectScreen.MenuSystem.GetSubSystem<Input.InputSystem>().CurrentInput[2], textsection, "p2", Grid.MoveOverEmptyBoxes);
            m_p1TeamInfo = new TeamSelectData(selectScreen, selectScreen.MenuSystem.GetSubSystem<Input.InputSystem>().CurrentInput[1], textsection, "p1", Grid.MoveOverEmptyBoxes);
            m_p2TeamInfo = new TeamSelectData(selectScreen, selectScreen.MenuSystem.GetSubSystem<Input.InputSystem>().CurrentInput[2], textsection, "p2", Grid.MoveOverEmptyBoxes);
        }

        public void SetInput(Input.InputState inputstate)
        {
            inputstate[0].Add(SystemButton.Quit, BackToTitleScreen);

            SetTeamModeCharacterSelectionInput(m_p1TeamInfo);
            SetTeamModeCharacterSelectionInput(m_p2TeamInfo);
        }

        public void Reset()
        {
            m_p1info.Reset();
            m_p2info.Reset();
            m_p1TeamInfo.Reset();
            m_p2TeamInfo.Reset();

            m_stageSelect.Reset();

            m_isdone = false;
        }

        public void Update(GameTime gametime)
        {
            m_p1info.Update();
            m_p2info.Update();

            m_p1TeamInfo.Update();
            m_p2TeamInfo.Update();

            CheckReady();
        }

        public void Draw(bool debugdraw)
        {
            Grid.Draw();
            var p1 = GetSelectData(m_p1TeamInfo);
            var p2 = GetSelectData(m_p2TeamInfo);
            Grid.DrawCursorGrid(p1, p2);
            DrawFace(p1);
            DrawFace(p2);
            m_stageSelect.Draw();
            SelectScreen.Print(m_titlefont, (Vector2)m_titlelocation, VersusMode, null);
            m_p1TeamInfo.Draw();
            m_p2TeamInfo.Draw();
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

        private void DrawFace(SelectData data)
        {
            if (data == null) return;

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
                m_stageSelect.SetStageSelectionInput(data.P1SelectData);
            }
            else if (data.State == TeamSelectState.SelectSelf)
            {
                data.State = TeamSelectState.SelectMate;
            }
            else
            {
                data.ButtonMap.Clear();
                m_stageSelect.SetStageSelectionInput(data.P2SelectData);
            }
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

        private void CheckReady()
        {
            if (m_isdone) return;
            if (m_stageSelect.IsSelected == false || m_p1TeamInfo.IsSelected == false || m_p2TeamInfo.IsSelected == false) return;

            m_isdone = true;

            var p11index = m_p1TeamInfo.P1SelectData.CurrentCell.Y * Grid.Size.X + m_p1TeamInfo.P1SelectData.CurrentCell.X;
            var p12index = m_p1TeamInfo.P2SelectData.CurrentCell.Y * Grid.Size.X + m_p1TeamInfo.P2SelectData.CurrentCell.X;
            var p21index = m_p2TeamInfo.P1SelectData.CurrentCell.Y * Grid.Size.X + m_p2TeamInfo.P1SelectData.CurrentCell.X;
            var p22index = m_p2TeamInfo.P2SelectData.CurrentCell.Y * Grid.Size.X + m_p2TeamInfo.P2SelectData.CurrentCell.X;

            var p11 = PlayerProfiles[p11index];
            var p12 = m_p1TeamInfo.TeamMode != TeamMode.Simul ? null : PlayerProfiles[p12index];
            var p21 = PlayerProfiles[p21index];
            var p22 = m_p2TeamInfo.TeamMode != TeamMode.Simul ? null : PlayerProfiles[p22index];

            var init = new Combat.EngineInitialization(CombatMode.TeamVersus,
                                                       m_p1TeamInfo.TeamMode,
                                                       m_p2TeamInfo.TeamMode,
                                                       p11.Profile, m_p1TeamInfo.P1SelectData.PaletteIndex, PlayerMode.Human,
                                                       p12?.Profile, m_p1TeamInfo.P2SelectData.PaletteIndex, m_p1TeamInfo.TeamMode == TeamMode.Simul ? PlayerMode.Ai : PlayerMode.Human,
                                                       p21.Profile, m_p2TeamInfo.P1SelectData.PaletteIndex, PlayerMode.Human,
                                                       p22?.Profile, m_p2TeamInfo.P2SelectData.PaletteIndex, m_p2TeamInfo.TeamMode == TeamMode.Simul ? PlayerMode.Ai : PlayerMode.Human,
                                                       m_stageSelect.CurrentStage);

            SelectScreen.MenuSystem.PostEvent(new Events.SetupCombat(init));
            SelectScreen.MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Versus));
        }

        private void BackToTitleScreen(bool pressed)
        {
            if (pressed)
            {
                SelectScreen.SoundManager.Play(m_soundcancel);

                SelectScreen.MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Title));
            }
        }

        public string VersusMode => "Team Versus";

        private ListIterator<PlayerSelect> PlayerProfiles => SelectScreen.MenuSystem.GetSubSystem<ProfileLoader>().PlayerProfiles;

        private SelectGrid Grid => SelectScreen.Grid;

        private SelectScreen SelectScreen { get; }

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly StageSelect m_stageSelect;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly SoundId m_soundcancel;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Point m_titlelocation;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly PrintData m_titlefont;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly SelectData m_p1info;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly SelectData m_p2info;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool m_isdone;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TeamSelectData m_p1TeamInfo;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private TeamSelectData m_p2TeamInfo;

        #endregion
    }
}