using Microsoft.Xna.Framework;

using NePlus.EngineComponents;

namespace NePlus.GameComponents
{
    public class InputComponent
    {
        public Input Input { get; private set; }

        public InputComponent(Game game)
        {
            Input = game.Engine.Input;
        }
    }
}
