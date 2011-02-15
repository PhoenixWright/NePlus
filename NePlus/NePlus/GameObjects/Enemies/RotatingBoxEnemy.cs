using Microsoft.Xna.Framework;

using NePlus.Components.GameComponents;
using NePlus.Components.GraphicsComponents;

namespace NePlus.GameObjects.Enemies
{
    public class RotatingBoxEnemy : Enemy
    {
        public RotatingBoxEnemy(Engine engine, Vector2 position)
            : base(engine, position)
        {
            animation = new Animation(engine, @"Characters\GrayRotatingBox", 128, 128, 4, 4, 16, 9);
            animation.DrawOrder = int.MaxValue - 1;

            engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            // manipulate the physics object to float around and make dives at the player

            base.Update(gameTime);
        }
    }
}
