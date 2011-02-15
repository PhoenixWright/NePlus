using Microsoft.Xna.Framework;

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

            engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            // if the bullets aren't in view anymore then get rid of them
            if (!Engine.Camera.VisibleArea.Contains((int)bulletPhysicsComponent.Position.X, (int)bulletPhysicsComponent.Position.Y))
            {
                Dispose(true);
                Engine.RemoveComponent(this);
            }

            base.Update(gameTime);
        }

        public override void Dispose(bool disposing)
        {
            bulletPhysicsComponent.Dispose(disposing);
            bulletPhysicsComponent = null;

            base.Dispose(disposing);
        }
    }
}
