using Microsoft.Xna.Framework;

namespace NePlus.GameObjects.Enemies
{
    public class RollingCircleEnemy : Enemy
    {
        public RollingCircleEnemy(Engine engine, Vector2 position, Global.Shapes shape)
            : base(engine, position, shape)
        {
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
