using Microsoft.Xna.Framework;

namespace xnaMugen.Menus
{
    interface ISelectScreenBehavior
    {
        void SetInput(Input.InputState inputstate);
        void Reset();
        void Update(GameTime gametime);
        void Draw(bool debugdraw);

        string VersusMode { get; }
    }
}