using Microsoft.Xna.Framework;
using xnaMugen.Drawing;
using xnaMugen.IO;

namespace xnaMugen.Menus
{
    internal class OptionsScreen : NonCombatScreen
    {
        public OptionsScreen(MenuSystem menusystem, TextSection titleSection, TextSection textsection, string spritepath, string animationpath, string soundpath) : base(menusystem, textsection, spritepath, animationpath, soundpath)
        {
            m_mainFont = titleSection.GetAttribute<PrintData>("menu.item.font");
            m_soundCancel = textsection.GetAttribute<SoundId>("cancel.snd");
        }

        public override void Draw(bool debugdraw)
        {
            base.Draw(debugdraw);

            Print(m_mainFont, new Microsoft.Xna.Framework.Vector2(160, 20), "Options", null);
        }

        public override void SetInput(Input.InputState inputstate)
        {
            base.SetInput(inputstate);

            inputstate[0].Add(SystemButton.Quit, BackToTitleScreen);
        }

        private void BackToTitleScreen(bool pressed)
        {
            if (pressed)
            {
                SoundManager.Play(m_soundCancel);

                MenuSystem.PostEvent(new Events.SwitchScreen(ScreenType.Title));
            }
        }

        private SoundId m_soundCancel;
        private PrintData m_mainFont;
    }
}