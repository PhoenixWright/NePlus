using Microsoft.Xna.Framework;

namespace NePlus.GameComponents
{
    public class InputComponent
    {
        Engine engine;

        public InputComponent(Game game)
        {
            engine = game.Engine;
        }

        public bool Jump()
        {
            return engine.Input.IsCurPress(engine.Configuration.JumpButton) || engine.Input.IsCurPress(engine.Configuration.JumpKey);
        }
    }
}
