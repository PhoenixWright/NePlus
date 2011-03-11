using System;

using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace NePlus.Components.PhysicsComponents
{
    public class BulletPhysicsComponent : PhysicsComponent
    {
        public BulletPhysicsComponent(Engine engine, Vector2 initialGameWorldPosition, Vector2 directionalVector, float angle, Global.CollisionCategories category)
            : base(engine)
        {
            MainFixture = FixtureFactory.CreateCircle(Engine.Physics.World, 0.05f, 1.0f);
            Bodies.Add(MainFixture.Body);
            MainFixture.Body.Rotation = angle;
            MainFixture.Body.BodyType = BodyType.Dynamic;
            MainFixture.Body.IgnoreGravity = true;
            MainFixture.Body.IsBullet = true;
            MainFixture.Body.Position = Engine.Physics.PositionToPhysicsWorld(initialGameWorldPosition);
            MainFixture.CollisionFilter.CollisionCategories = (Category)category;

            MainFixture.Body.LinearVelocity = directionalVector;
        }

        public override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
        }
    }
}
