using System.Diagnostics;
using xnaMugen.IO;
using xnaMugen.Drawing;
using xnaMugen.Collections;
using Microsoft.Xna.Framework;
using System.Text;
using System;

namespace xnaMugen.Menus
{
    internal class StageSelect
    {
        public StageSelect(SelectScreen selectScreen, TextSection textsection)
        {
            SelectScreen = selectScreen;
            m_stageposition = textsection.GetAttribute<Point>("stage.pos");
            m_soundstagemove = textsection.GetAttribute<SoundId>("stage.move.snd");
            m_soundstageselect = textsection.GetAttribute<SoundId>("stage.done.snd");
            m_stagefont1 = textsection.GetAttribute<PrintData>("stage.active.font");
            m_stagefont2 = textsection.GetAttribute<PrintData>("stage.active2.font");
            m_stagedonefont = textsection.GetAttribute<PrintData>("stage.done.font");
            m_stagedisplaybuilder = new StringBuilder();
        }

        public void Reset()
        {
            m_blinkval = 0;
            m_stageselected = false;
            StageSelector = null;
            CurrentStageIndex = -1;
        }

        public void Update()
        {
            if (++m_blinkval > 6) m_blinkval = -6;
        }

        public void Draw()
        {
            if (StageSelector == null) return;

            var pd = m_blinkval > 0 ? m_stagefont1 : m_stagefont2;
            if (m_stageselected) pd = m_stagedonefont;

            var sp = CurrentStageIndex >= 0 && CurrentStageIndex < StageProfiles.Count ? StageProfiles[CurrentStageIndex] : null;

            m_stagedisplaybuilder.Length = 0;
            if (sp != null)
            {
                m_stagedisplaybuilder.AppendFormat("Stage {0}: {1}", CurrentStageIndex + 1, sp.Name);
            }
            else
            {
                m_stagedisplaybuilder.Append("Stage: Random");
            }

            SelectScreen.Print(pd, (Vector2)m_stageposition, m_stagedisplaybuilder.ToString(), null);
        }

        public void SetStageSelectionInput(SelectData data)
        {
            if (data == null) throw new ArgumentNullException(nameof(data));

            if (StageSelector != null) return;

            data.ButtonMap.Clear();

            data.ButtonMap.Add(PlayerButton.Left, delegate (bool pressed) { if (pressed) { MoveStageSelection(-1); } });
            data.ButtonMap.Add(PlayerButton.Right, delegate (bool pressed) { if (pressed) { MoveStageSelection(+1); } });
            data.ButtonMap.Add(PlayerButton.A, SelectCurrentStage);
            data.ButtonMap.Add(PlayerButton.B, SelectCurrentStage);
            data.ButtonMap.Add(PlayerButton.C, SelectCurrentStage);
            data.ButtonMap.Add(PlayerButton.X, SelectCurrentStage);
            data.ButtonMap.Add(PlayerButton.Y, SelectCurrentStage);
            data.ButtonMap.Add(PlayerButton.Z, SelectCurrentStage);

            StageSelector = data;
            CurrentStageIndex = 0;
        }

        private void MoveStageSelection(int offset)
        {
            if (offset == 0) return;
            SelectScreen.SoundManager.Play(m_soundstagemove);

            offset = offset % StageProfiles.Count;

            if (offset > 0)
            {
                CurrentStageIndex += offset;
                if (CurrentStageIndex >= StageProfiles.Count)
                {
                    var diff = CurrentStageIndex - StageProfiles.Count;
                    CurrentStageIndex = -1 + diff;
                }
            }

            if (offset < 0)
            {
                CurrentStageIndex += offset;
                if (CurrentStageIndex < -1)
                {
                    var diff = CurrentStageIndex + 2;
                    CurrentStageIndex = StageProfiles.Count - 1 + diff;
                }
            }
        }

        private void SelectCurrentStage(bool pressed)
        {
            if (pressed)
            {
                m_stageselected = true;
                SelectScreen.SoundManager.Play(m_soundstageselect);

                StageSelector.ButtonMap.Clear();
            }
        }

        public SelectScreen SelectScreen { get; }

        public int CurrentStageIndex { get; set; }

        public StageProfile CurrentStage
        {
            get
            {
                var index = CurrentStageIndex;
                if (index == -1) index = SelectScreen.MenuSystem.GetSubSystem<Random>().NewInt(0, StageProfiles.Count - 1);
                return StageProfiles[index];
            }
        }

        public bool IsSelected => m_stageselected;

        public SelectData StageSelector { get; set; }

        private ListIterator<StageProfile> StageProfiles => SelectScreen.MenuSystem.GetSubSystem<ProfileLoader>().StageProfiles;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly SoundId m_soundstagemove;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly SoundId m_soundstageselect;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly Point m_stageposition;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly PrintData m_stagefont1;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly PrintData m_stagefont2;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly PrintData m_stagedonefont;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly StringBuilder m_stagedisplaybuilder;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private bool m_stageselected;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private int m_blinkval;
    }
}