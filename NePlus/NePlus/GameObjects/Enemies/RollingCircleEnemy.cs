using Microsoft.Xna.Framework;

using NePlus.Components.GameComponents;

namespace NePlus.GameObjects.Enemies
{
    public class RollingCircleEnemy : Enemy
    {
        public RollingCircleEnemy(Engine engine, Vector2 position, Global.Shapes shape)
            : base(engine, position, shape)
        {
            animation = new Animation(engine, @"Characters\FlickeringCircle", 64, 64, 1, 2, 2, 15, Global.Animations.Repeat);
            animation.Play();

            engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (enemyPhysicsComponent.Position.X > Engine.Player.Position.X)
            {
                enemyPhysicsComponent.MainFixture.Body.ApplyTorque(-5.0f);
            }
            else if (enemyPhysicsComponent.Position.X < Engine.Player.Position.X)
            {
                enemyPhysicsComponent.MainFixture.Body.ApplyTorque(5.0f);
            }

            base.Update(gameTime);
        }
    }
}
