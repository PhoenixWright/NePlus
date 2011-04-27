using System;

using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;

namespace NePlus.Components.PhysicsComponents
{
    public class EnemyPhysicsComponent : PhysicsComponent
    {
        public EnemyPhysicsComponent(Engine engine, Vector2 gameWorldPosition, Global.Shapes shape)
            : base(engine)
        {
            switch (shape)
            {
                case Global.Shapes.Circle:
                    MainFixture = FixtureFactory.CreateCircle(Engine.Physics.World, 0.25f, 1.0f);
                    MainFixture.Friction = 0.5f;
                    break;
                case Global.Shapes.Player:
                    MainFixture = FixtureFactory.CreateRectangle(Engine.Physics.World, 0.5f, 0.5f, 1);
                    Bodies.Add(MainFixture.Body);
                    MainFixture.Body.Position = Engine.Physics.PositionToPhysicsWorld(gameWorldPosition);
                    MainFixture.Body.BodyType = BodyType.Dynamic;
                    MainFixture.Body.SleepingAllowed = false;

                    Fixture WheelFixture = FixtureFactory.CreateCircle(Engine.Physics.World, 0.3f, 1.0f);
                    Bodies.Add(WheelFixture.Body);
                    WheelFixture.Body.Position = MainFixture.Body.Position + new Vector2(0.0f, 0.6f);
                    WheelFixture.Body.BodyType = BodyType.Dynamic;

                    WheelFixture.Body.SleepingAllowed = false;
                    WheelFixture.Friction = 0.5f;

                    FixedAngleJoint playerFAJ = JointFactory.CreateFixedAngleJoint(Engine.Physics.World, MainFixture.Body);
                    playerFAJ.BodyB = WheelFixture.Body;

                    RevoluteJoint wheelMotorRevJoint = JointFactory.CreateRevoluteJoint(MainFixture.Body, WheelFixture.Body, Vector2.Zero);
                    wheelMotorRevJoint.MaxMotorTorque = 10.0f;
                    wheelMotorRevJoint.MotorEnabled = true;
                    Engine.Physics.World.AddJoint(wheelMotorRevJoint);
                    break;
                case Global.Shapes.Square:
                    MainFixture = FixtureFactory.CreateRectangle(Engine.Physics.World, 0.5f, 0.5f, 1.0f);
                    break;
                default:
                    throw new Exception("shape " + shape.ToString() + " not recognized when creating EnemyPhysicsComponent");
            }

            Bodies.Add(MainFixture.Body);
            MainFixture.Body.Position = Engine.Physics.PositionToPhysicsWorld(gameWorldPosition);
            MainFixture.Body.BodyType = BodyType.Dynamic;
        }
    }
}
