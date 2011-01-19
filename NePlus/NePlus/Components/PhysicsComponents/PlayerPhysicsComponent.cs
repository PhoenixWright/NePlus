using Microsoft.Xna.Framework;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using NePlus.ScreenManagement;
using NePlus.ScreenManagement.Screens;

namespace NePlus.Components.PhysicsComponents
{
    public class PlayerPhysicsComponent : PhysicsComponent
    {
        RevoluteJoint wheelMotorRevJoint;

        public PlayerPhysicsComponent(Engine engine, Vector2 gameWorldPosition, bool dynamic)
            : base(engine)
        {
            MainFixture = FixtureFactory.CreateRectangle(Engine.Physics.World, 0.5f, 0.5f, 1);
            MainFixture.CollisionFilter.CollisionGroup = 1;
            MainFixture.Body.Position = Engine.Physics.PositionToPhysicsWorld(gameWorldPosition);
            if (dynamic)
            {
                MainFixture.Body.BodyType = BodyType.Dynamic;
            }
            else
            {
                MainFixture.Body.BodyType = BodyType.Static;
            }
            MainFixture.Body.SleepingAllowed = false;

            Fixture wheelFixture = FixtureFactory.CreateCircle(Engine.Physics.World, 0.3f, 1.0f);
            wheelFixture.Body.Position = MainFixture.Body.Position + new Vector2(0.0f, 0.6f);
            wheelFixture.Body.BodyType = MainFixture.Body.BodyType;
            wheelFixture.Body.SleepingAllowed = false;
            wheelFixture.Friction = 0.3f;

            FixedAngleJoint playerFAJ = JointFactory.CreateFixedAngleJoint(Engine.Physics.World, MainFixture.Body);
            playerFAJ.BodyB = wheelFixture.Body;

            wheelMotorRevJoint = JointFactory.CreateRevoluteJoint(MainFixture.Body, wheelFixture.Body, Vector2.Zero);
            wheelMotorRevJoint.MaxMotorTorque = 100.0f;
            wheelMotorRevJoint.MotorEnabled = true;
            Engine.Physics.World.AddJoint(wheelMotorRevJoint);
        }
    }
}