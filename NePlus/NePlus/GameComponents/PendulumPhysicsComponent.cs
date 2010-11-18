using Microsoft.Xna.Framework;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace NePlus.GameComponents
{
    public class PendulumPhysicsComponent : PhysicsComponent
    {
        private Fixture pivotFixture;

        public PendulumPhysicsComponent(Point gameWorldPivotPosition, Point gameWorldWeightPosition)
        {
            pivotFixture = FixtureFactory.CreateCircle(Engine.Physics.World, 0.3f, 1.0f, Engine.Physics.PositionToPhysicsWorld(new Vector2(gameWorldPivotPosition.X, gameWorldPivotPosition.Y)));
            MainFixture = FixtureFactory.CreateCircle(Engine.Physics.World, 0.5f, 1.0f, Engine.Physics.PositionToPhysicsWorld(new Vector2(gameWorldWeightPosition.X, gameWorldWeightPosition.Y)));

            MainFixture.Body.BodyType = BodyType.Dynamic;

            JointFactory.CreateDistanceJoint(Engine.Physics.World, pivotFixture.Body, MainFixture.Body, pivotFixture.Body.Position, MainFixture.Body.Position);
        }
    }
}
