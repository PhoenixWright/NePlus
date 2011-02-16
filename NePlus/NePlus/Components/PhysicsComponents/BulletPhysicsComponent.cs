using System;

using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace NePlus.Components.PhysicsComponents
{
    public class BulletPhysicsComponent : PhysicsComponent
    {
        private float bulletVelocityX;

        public BulletPhysicsComponent(Engine engine, Vector2 initialGameWorldPosition, Global.Directions direction, Global.CollisionCategories category)
            : base(engine)
        {
            bulletVelocityX = Global.Configuration.GetFloatConfig("Physics", "BulletVelocityX");

            MainFixture = FixtureFactory.CreateCircle(Engine.Physics.World, 0.05f, 1.0f);
            Bodies.Add(MainFixture.Body);
            MainFixture.Body.BodyType = BodyType.Dynamic;
            MainFixture.Body.IgnoreGravity = true;
            MainFixture.Body.IsBullet = true;
            MainFixture.Body.Position = Engine.Physics.PositionToPhysicsWorld(initialGameWorldPosition);
            MainFixture.CollisionFilter.CollisionCategories = (Category)category;

            Vector2 directionVector;

            // decide what velocity vector to assign based on direction
            switch (direction)
            {
                case Global.Directions.Left:
                    directionVector = new Vector2(-bulletVelocityX, 0.0f);
                    break;
                case Global.Directions.Right:
                    directionVector = new Vector2(bulletVelocityX, 0.0f);
                    break;
                default:
                    throw new Exception("direction " + direction.ToString() + " not handled in BulletPhysicsComponent");
            }

            MainFixture.Body.LinearVelocity = directionVector;
        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
