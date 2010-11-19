using Microsoft.Xna.Framework;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

namespace NePlus.GameComponents
{
    public class PendulumPhysicsComponent : PhysicsComponent
    {
        private Fixture pivotFixture;
        DistanceJoint distanceJoint;

        public PendulumPhysicsComponent(Point gameWorldPivotPosition, Point gameWorldWeightPosition)
        {
            pivotFixture = FixtureFactory.CreateRectangle(Engine.Physics.World, 0.8f, 0.8f, 1.0f);
            MainFixture = FixtureFactory.CreateRectangle(Engine.Physics.World, 0.8f, 0.8f, 1.0f);
            MainFixture.Body.Mass = 100.0f;

            pivotFixture.Body.Position = Engine.Physics.PositionToPhysicsWorld(new Vector2(gameWorldPivotPosition.X, gameWorldPivotPosition.Y));
            MainFixture.Body.Position = Engine.Physics.PositionToPhysicsWorld(new Vector2(gameWorldWeightPosition.X, gameWorldWeightPosition.Y));

            pivotFixture.CollidesWith = CollisionCategory.Cat31;
            MainFixture.Body.BodyType = BodyType.Dynamic;

            distanceJoint = JointFactory.CreateDistanceJoint(Engine.Physics.World, pivotFixture.Body, MainFixture.Body, pivotFixture.Body.Position, MainFixture.Body.Position);
        }
    }
}
