using Microsoft.Xna.Framework;

namespace xnaMugen.Menus
{
    internal class StoryboardScreen : Screen
    {
        public Storyboard Storyboard { get; set; }
        public Events.Base Event { get; set; }

        public StoryboardScreen(MenuSystem menusystem)
            : base(menusystem)
        {
        }

        public override int FadeInTime => 1;

        public override int FadeOutTime => 1;

		public override void Draw(bool debugdraw)
        {
            Storyboard?.Draw();
        }

        public override void Update(GameTime gametime)
        {
            Storyboard?.Update();
            if (Storyboard?.IsFinished==true)
            {
                MenuSystem.PostEvent(Event);
            }
        }

	}
}