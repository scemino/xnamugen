using System;
using System.Diagnostics;
using xnaMugen.IO;
using Microsoft.Xna.Framework;

namespace xnaMugen.Menus
{
    internal class VsModeSelectScreen : SelectScreen
    {
        public VsModeSelectScreen(MenuSystem screensystem, TextSection textsection, string spritepath, string animationpath, string soundpath)
            : base(screensystem, textsection, spritepath, animationpath, soundpath)
        {
            VersusMode = "Versus Mode";
        }

        public override void SetInput(Input.InputState inputstate)
        {
            base.SetInput(inputstate);

            inputstate[0].Add(SystemButton.Quit, BackToTitleScreen);

            SetCharacterSelectionInput(m_p1info);
            SetCharacterSelectionInput(m_p2info);
        }

        public override void Reset()
        {
            base.Reset();

            m_blinkval = 0;
            m_p1info.Reset();
            m_p2info.Reset();

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

            CheckReady();
        }

        public override void Draw(bool debugdraw)
        {
            base.Draw(debugdraw);

            DrawGrid();
            DrawFace(m_p1info);
            DrawFace(m_p2info);
            DrawStage();
            Print(m_titlefont, (Vector2)m_titlelocation, m_title, null);
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

            if (data.MoveCharacterCursor(direction, m_gridsize, m_wrapping))
            {
                data.PlayCursorMoveSound();
            }
        }

        private void SelectCharacter(SelectData data, int index)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            var selection = GetSelection(data.CurrentCell, false);
            if (selection == null || selection.SelectionType != PlayerSelectType.Profile) return;

            data.PlaySelectSound();
            data.ButtonMap.Clear();
            data.IsSelected = true;
            data.PaletteIndex += index;

            SetStageSelectionInput(data);
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
            if (m_isdone) return;
            if (m_stageselected == false || m_p1info.IsSelected == false || m_p2info.IsSelected == false) return;

            m_isdone = true;

            if (m_currentstage == -1) m_currentstage = MenuSystem.GetSubSystem<Random>().NewInt(0, StageProfiles.Count - 1);

            var p1index = m_p1info.CurrentCell.Y * m_gridsize.X + m_p1info.CurrentCell.X;
            var p2index = m_p2info.CurrentCell.Y * m_gridsize.X + m_p2info.CurrentCell.X;

            var p1 = PlayerProfiles[p1index];
            var p2 = PlayerProfiles[p2index];
            var stage = StageProfiles[m_currentstage];

            var init = new Combat.EngineInitialization(CombatMode.Versus, p1.Profile, m_p1info.PaletteIndex, p2.Profile, m_p2info.PaletteIndex, stage);

            MenuSystem.PostEvent(new Events.SetupCombat(init));
            MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Versus));
        }

        private void BackToTitleScreen(bool pressed)
        {
            if (pressed)
            {
                SoundManager.Play(m_soundcancel);

                MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Title));
            }
        }

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_blinkval;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_currentstage;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool m_stageselected;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private SelectData m_stageselector;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool m_isdone;

        #endregion
    }
}