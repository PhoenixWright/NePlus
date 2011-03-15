using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using NePlus.Components.GameComponents;
using NePlus.Components.PhysicsComponents;

namespace NePlus.GameObjects
{
    public class Enemy : Component
    {
        // audio
        protected AudioEmitter audioEmitter;
        protected Cue enemySound;

        // components
        protected Animation animation;
        protected EnemyPhysicsComponent enemyPhysicsComponent;

        public int Health { get; protected set; }

        public Enemy(Engine engine, Vector2 position, Global.Shapes shape)
            : base(engine)
        {
            audioEmitter = new AudioEmitter();

            enemyPhysicsComponent = new EnemyPhysicsComponent(engine, position, shape);
            enemyPhysicsComponent.MainFixture.Body.LinearDamping = 2.0f;
            enemyPhysicsComponent.MainFixture.OnCollision += EnemyOnCollision;
            enemyPhysicsComponent.MainFixture.CollisionFilter.CollisionCategories = (Category)Global.CollisionCategories.Enemy;

            Health = 100;

            engine.AddComponent(this);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            audioEmitter.Position = new Vector3(enemyPhysicsComponent.Position, 0);

            if (Health <= 0)
            {
                Dispose(true);
            }

            if (animation != null && enemyPhysicsComponent != null)
            {
                animation.Position = enemyPhysicsComponent.Position;
                animation.Angle = enemyPhysicsComponent.Angle;
            }

            base.Update(gameTime);
        }

        private bool EnemyOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            // check if the player collided with the enemy
            if (fixtureB.CollisionFilter.IsInCollisionCategory((Category)Global.CollisionCategories.Player))
            {
                // player collided with enemy
                OnEnemyPlayerCollision();
            }

            // check if a bullet hit the enemy
            if (fixtureB.CollisionFilter.IsInCollisionCategory((Category)Global.CollisionCategories.PlayerBullet))
            {
                // bullet hit enemy
                Health = 0;
            }

            return true;
        }

        public override void Dispose(bool disposing)
        {
            if (enemySound != null)
            {
                enemySound.Stop(AudioStopOptions.Immediate);
                enemySound.Dispose();
                enemySound = null;
            }

            if (animation != null)
            {
                animation.Dispose(true);
                animation = null;
            }

            enemyPhysicsComponent.Dispose(true);
            enemyPhysicsComponent = null;

            base.Dispose(disposing);
        }

        protected virtual void OnEnemyPlayerCollision() { }
    }
}
