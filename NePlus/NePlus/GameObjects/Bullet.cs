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
        private BulletPhysicsComponent bulletPhysicsComponent;
        private Sprite bulletSprite;
        private Light light;

        private bool collided;
        Animation flashAnimation;
        Light flashLight;

        public Bullet(Engine engine, Vector2 position, Vector2 direction, float angle, Global.CollisionCategories category)
            : base(engine)
        {
            DrawOrder = (int)Global.Layers.Projectiles;

            bulletPhysicsComponent = new BulletPhysicsComponent(engine, position, direction, angle, category);
            bulletPhysicsComponent.MainFixture.OnCollision += BulletOnCollision;

            bulletSprite = new Sprite(engine, @"Miscellaneous\RedBullet");
            bulletSprite.DrawOrder = DrawOrder;

            light = new Light(engine);
            light.Color = Color.Red;
            light.Fov = MathHelper.TwoPi;
            light.Intensity = 0.8f;
            light.Position = bulletPhysicsComponent.Position;
            light.Range = 200;
            light.ShadowType = Krypton.Lights.ShadowType.Illuminated;

            switch (category)
            {
                case Global.CollisionCategories.PlayerBullet:
                    bulletPhysicsComponent.MainFixture.CollisionFilter.CollidesWith = (Category)(Global.CollisionCategories.Enemy | Global.CollisionCategories.EnemyBullet | Global.CollisionCategories.Light | Global.CollisionCategories.Structure);
                    break;
                default:
                    // do nothing
                    break;
            }

            collided = false;

            flashAnimation = new Animation(Engine, @"Miscellaneous\flash", 512, 512, 3, 3, 6, 20, Global.Animations.PlayOnce);
            flashAnimation.Scale = 0.4f;
            flashAnimation.Position = bulletPhysicsComponent.Position;
            flashAnimation.DrawOrder = int.MaxValue - 1;

            flashLight = new Light(engine);
            flashLight.Color = Color.Yellow;
            flashLight.Fov = MathHelper.TwoPi;
            flashLight.Intensity = 0.8f;
            flashLight.Position = bulletPhysicsComponent.Position;
            flashLight.Range = 100;
            flashLight.ShadowType = Krypton.Lights.ShadowType.Illuminated;
            flashLight.IsOn = false;

            engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (collided)
            {
                // turn off the normal light and turn on the light from the bullet flash
                if (light.IsOn)
                {
                    bulletSprite.Visible = false;
                    light.IsOn = false;
                    flashLight.IsOn = true;
                    flashAnimation.Play();
                }

                if (flashAnimation.Progress > 0.5f)
                {
                    flashLight.IsOn = false;
                }

                if (!flashAnimation.Playing)
                {
                    // animation over, turn off the flash light and dispose
                    Dispose(true);
                    return;
                }
            }
            else
            {
                bulletSprite.Position = bulletPhysicsComponent.Position;
                light.Position = bulletPhysicsComponent.Position;
                flashAnimation.Position = bulletPhysicsComponent.Position;
                flashLight.Position = bulletPhysicsComponent.Position;
            }

            // if the bullets aren't in view anymore then get rid of them
            if (!Engine.Camera.VisibleArea.Contains((int)bulletPhysicsComponent.Position.X, (int)bulletPhysicsComponent.Position.Y))
            {
                Dispose(true);
                return;
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

            flashAnimation.Dispose(true);
            flashAnimation = null;

            flashLight.Dispose(true);
            flashLight = null;

            base.Dispose(disposing);
        }

        private bool BulletOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            // kill the bullet on any collision except for lights
            if (!(fixtureB.CollisionFilter.CollisionCategories == (Category)Global.CollisionCategories.Light))
            {
                collided = true;
            }

            return true;
        }
    }
}
