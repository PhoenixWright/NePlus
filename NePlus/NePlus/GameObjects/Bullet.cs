using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using NePlus.Components.GameComponents;
using NePlus.Components.PhysicsComponents;

using NePlus.GameObjects.LightObjects;

namespace NePlus.GameObjects
{
    public class Bullet : Component
    {
        BulletPhysicsComponent bulletPhysicsComponent;
        Sprite bulletSprite;
        Light light;

        public Bullet(Engine engine, Vector2 position, Vector2 direction, float angle, Global.CollisionCategories category)
            : base(engine)
        {
            bulletPhysicsComponent = new BulletPhysicsComponent(engine, position, direction, angle, category);
            bulletPhysicsComponent.MainFixture.OnCollision += BulletOnCollision;

            bulletSprite = new Sprite(engine, @"Miscellaneous\RedBullet");

            light = new Light(engine);
            light.Color = Color.Red;
            light.Fov = MathHelper.TwoPi;
            light.Intensity = 0.8f;
            light.Position = bulletPhysicsComponent.Position;
            light.Range = 200;

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
            bulletSprite.Position = bulletPhysicsComponent.Position;
            light.Position = bulletPhysicsComponent.Position;

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

            bulletSprite.Dispose(true);
            bulletSprite = null;

            light.Dispose(true);
            light = null;

            base.Dispose(disposing);
        }

        private bool BulletOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            // kill the bullet on any collision except for lights
            if (!(fixtureB.CollisionFilter.CollisionCategories == (Category)Global.CollisionCategories.Light))
            {
                // TODO: play an animation before disposing
                Dispose(true);
            }

            return true;
        }
    }
}
