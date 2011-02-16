using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using NePlus.Components.PhysicsComponents;

namespace NePlus.GameObjects
{
    public class Bullet : Component
    {
        BulletPhysicsComponent bulletPhysicsComponent;

        public Bullet(Engine engine, Vector2 position, Global.Directions direction, Global.CollisionCategories category)
            : base(engine)
        {
            bulletPhysicsComponent = new BulletPhysicsComponent(engine, position, direction, category);
            bulletPhysicsComponent.MainFixture.OnCollision += BulletOnCollision;

            switch (category)
            {
                case Global.CollisionCategories.PlayerBullet:
                    bulletPhysicsComponent.MainFixture.CollisionFilter.CollidesWith = (Category)(Global.CollisionCategories.Enemy | Global.CollisionCategories.EnemyBullet | Global.CollisionCategories.Light | Global.CollisionCategories.Structure);
                    break;
                default:
                    // do nothing
                    break;
            }

            engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            // if the bullets aren't in view anymore then get rid of them
            if (!Engine.Camera.VisibleArea.Contains((int)bulletPhysicsComponent.Position.X, (int)bulletPhysicsComponent.Position.Y))
            {
                Dispose(true);
            }

            base.Update(gameTime);
        }

        public override void Dispose(bool disposing)
        {
            if (bulletPhysicsComponent != null)
            {
                bulletPhysicsComponent.Dispose(disposing);
                bulletPhysicsComponent = null;
            }

            base.Dispose(disposing);
        }

        private bool BulletOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            // kill the bullet on any collision except for lights
            if (!(fixtureB.CollisionFilter.CollisionCategories == (Category)Global.CollisionCategories.Light))
            {
                // TODO: play an animation first
                Dispose(true);
            }

            return true;
        }
    }
}
