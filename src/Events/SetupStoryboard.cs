using System;
using xnaMugen.Menus;

namespace xnaMugen.Events
{
    internal class SetupStoryboard : Base
    {
        public SetupStoryboard(Storyboard storyboard, Base @event)
        {
            if (storyboard == null) throw new ArgumentNullException(nameof(storyboard));

            Storyboard = storyboard;
            Event = @event;
        }

        public Storyboard Storyboard { get; }
        public Base Event { get; }
    }
}