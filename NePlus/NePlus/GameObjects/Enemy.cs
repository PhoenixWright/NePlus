using Microsoft.Xna.Framework;

using NePlus.Components.GameComponents;
using NePlus.Components.PhysicsComponents;

namespace NePlus.GameObjects
{
    public class Enemy : Component
    {
        // components
        protected Animation animation;
        protected EnemyPhysicsComponent enemyPhysicsComponent;

        public Enemy(Engine engine, Vector2 position)
            : base(engine)
        {
            enemyPhysicsComponent = new EnemyPhysicsComponent(engine, position);
            enemyPhysicsComponent.MainFixture.Body.LinearDamping = 2.0f;

            engine.AddComponent(this);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (animation != null && enemyPhysicsComponent != null)
            {
                animation.Position = enemyPhysicsComponent.Position;
                animation.Angle = enemyPhysicsComponent.Angle;
            }

            base.Update(gameTime);
        }
    }
}
