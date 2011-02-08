using Microsoft.Xna.Framework;

using FarseerPhysics.Common;
using FarseerPhysics.Controllers;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

using NePlus.ScreenManagement;
using NePlus.ScreenManagement.Screens;

namespace NePlus.Components.PhysicsComponents
{
    public class PlayerPhysicsComponent : PhysicsComponent
    {
        public Fixture WheelFixture;
        RevoluteJoint wheelMotorRevJoint;

        public PlayerPhysicsComponent(Engine engine, Vector2 gameWorldPosition, bool dynamic)
            : base(engine)
        {
            MainFixture = FixtureFactory.CreateRectangle(Engine.Physics.World, 0.5f, 0.5f, 1);
            Bodies.Add(MainFixture.Body);
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

            WheelFixture = FixtureFactory.CreateCircle(Engine.Physics.World, 0.3f, 1.0f);
            Bodies.Add(WheelFixture.Body);
            WheelFixture.Body.Position = MainFixture.Body.Position + new Vector2(0.0f, 0.6f);
            WheelFixture.Body.BodyType = BodyType.Dynamic;

            WheelFixture.Body.SleepingAllowed = false;
            WheelFixture.Friction = 0.5f;

            FixedAngleJoint playerFAJ = JointFactory.CreateFixedAngleJoint(Engine.Physics.World, MainFixture.Body);
            playerFAJ.BodyB = WheelFixture.Body;

            wheelMotorRevJoint = JointFactory.CreateRevoluteJoint(MainFixture.Body, WheelFixture.Body, Vector2.Zero);
            wheelMotorRevJoint.MaxMotorTorque = 10.0f;
            wheelMotorRevJoint.MotorEnabled = true;
            Engine.Physics.World.AddJoint(wheelMotorRevJoint);
        }

        public void MoveLeft()
        {
            wheelMotorRevJoint.MotorSpeed = -7;
        }

        public void MoveRight()
        {
            wheelMotorRevJoint.MotorSpeed = 7;
        }

        public void StopMoving()
        {
            wheelMotorRevJoint.MotorSpeed = 0;
        }
    }
}