using System;
using System.Diagnostics;
using xnaMugen.IO;
using xnaMugen.Drawing;
using xnaMugen.Collections;
using Microsoft.Xna.Framework;

namespace xnaMugen.Menus
{
    internal class ArcadeSelectScreenBehavior : ISelectScreenBehavior
    {
        public ArcadeSelectScreenBehavior(SelectScreen selectScreen, TextSection textsection)
        {
            SelectScreen = selectScreen;

            m_stageSelect = new StageSelect(selectScreen, textsection);

            m_soundcancel = textsection.GetAttribute<SoundId>("cancel.snd");
            m_titlelocation = textsection.GetAttribute<Point>("title.offset");
            m_titlefont = textsection.GetAttribute<PrintData>("title.font");

            m_p1info = new SelectData(selectScreen, selectScreen.MenuSystem.GetSubSystem<Input.InputSystem>().CurrentInput[1], textsection, "p1", Grid.MoveOverEmptyBoxes);
        }

        public void SetInput(Input.InputState inputstate)
        {
            inputstate[0].Add(SystemButton.Quit, BackToTitleScreen);

            SetCharacterSelectionInput(m_p1info);
        }

        public void Reset()
        {
            m_p1info.Reset();

            m_stageSelect.Reset();
            m_isdone = false;
        }

        public void Update(GameTime gametime)
        {
            m_stageSelect.Update();
            m_p1info.Update();

            CheckReady();
        }

        public void Draw(bool debugdraw)
        {
            Grid.Draw();
            Grid.DrawCursorGrid(m_p1info, null);
            DrawFace(m_p1info);

            m_stageSelect.Draw();
            SelectScreen.Print(m_titlefont, (Vector2)m_titlelocation, VersusMode, null);
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

        private void MoveCharacterSelection(SelectData data, CursorDirection direction)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (data.MoveCharacterCursor(direction, Grid.Size, Grid.Wrapping))
            {
                data.PlayCursorMoveSound();
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

            m_stageSelect.SetStageSelectionInput(data);
        }

        private void CheckReady()
        {
            if (m_isdone) return;
            if (m_stageSelect.IsSelected == false || m_p1info.IsSelected == false) return;

            m_isdone = true;

            var p1index = m_p1info.CurrentCell.Y * Grid.Size.X + m_p1info.CurrentCell.X;

            var p1 = PlayerProfiles[p1index];
            var p2 = PlayerProfiles[0];

            var init = new Combat.EngineInitialization(CombatMode.Arcade,
                                                       p1.Profile, m_p1info.PaletteIndex, PlayerMode.Human,
                                                       p2.Profile, 0, PlayerMode.Ai,
                                                       m_stageSelect.CurrentStage);

            SelectScreen.MenuSystem.PostEvent(new Events.SetupCombat(init));

            var storyboardPath = p1.Profile.BasePath + "/intro.def";
            var storyboard = new Storyboard(SelectScreen.MenuSystem, storyboardPath);
            SelectScreen.MenuSystem.PostEvent(new Events.SetupStoryboard(storyboard,new Events.SwitchScreen(ScreenType.Versus)));
            SelectScreen.MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Storyboard));
        }

        private void BackToTitleScreen(bool pressed)
        {
            if (pressed)
            {
                SelectScreen.SoundManager.Play(m_soundcancel);
                SelectScreen.MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Title));
            }
        }

        public string VersusMode => "Arcade";

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
        private bool m_isdone;
        #endregion
    }
}