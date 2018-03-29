using System.Diagnostics;
using xnaMugen.IO;
using xnaMugen.Collections;
using Microsoft.Xna.Framework;
using xnaMugen.Elements;

namespace xnaMugen.Menus
{
    internal class SelectScreen : NonCombatScreen
    {
        public SelectScreen(MenuSystem screensystem, TextSection textsection, string spritepath, string animationpath, string soundpath)
            : base(screensystem, textsection, spritepath, animationpath, soundpath)
        {
            m_textsection = textsection;
            var elements = new Collection(SpriteManager, AnimationManager, SoundManager, MenuSystem.FontMap);
            Grid = new SelectGrid(textsection, elements, PlayerProfiles);
        }

        public override void SetInput(Input.InputState inputstate)
        {
            base.SetInput(inputstate);
            m_selectScreenBehavior.SetInput(inputstate);
        }

        public override void Reset()
        {
            base.Reset();
            m_selectScreenBehavior.Reset();
        }

        public override void Update(GameTime gametime)
        {
            base.Update(gametime);
            m_selectScreenBehavior.Update(gametime);
        }

        public override void Draw(bool debugdraw)
        {
            base.Draw(debugdraw);
            m_selectScreenBehavior.Draw(debugdraw);
        }

        public string VersusMode => m_selectScreenBehavior.VersusMode;

        public CombatMode CombatMode
        {
            get { return m_combatMode; }
            set
            {
                m_combatMode = value;
                switch (m_combatMode)
                {
                    case CombatMode.Arcade:
                        m_selectScreenBehavior = new ArcadeSelectScreenBehavior(this, m_textsection);
                        break;
                    case CombatMode.Versus:
                        m_selectScreenBehavior = new VersusSelectScreenBehavior(this, m_textsection);
                        break;
                    case CombatMode.TeamArcade:
                        m_selectScreenBehavior = new TeamArcadeSelectScreenBehavior(this, m_textsection);
                        break;
                    case CombatMode.TeamVersus:
                        m_selectScreenBehavior = new TeamSelectScreenBehavior(this, m_textsection);
                        break;
                }
            }
        }

        public ListIterator<PlayerSelect> PlayerProfiles => MenuSystem.GetSubSystem<ProfileLoader>().PlayerProfiles;

        public SelectGrid Grid { get; }

        #region Fields

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private CombatMode m_combatMode;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private readonly TextSection m_textsection;

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private ISelectScreenBehavior m_selectScreenBehavior;

        #endregion
    }
}