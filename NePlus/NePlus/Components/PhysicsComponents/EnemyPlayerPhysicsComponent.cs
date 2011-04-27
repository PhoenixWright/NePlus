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
    public class EnemyPlayerPhysicsComponent : PhysicsComponent
    {
        public Fixture WheelFixture;
        private FixedAngleJoint playerFAJ;
        private RevoluteJoint wheelMotorRevJoint;

        public EnemyPlayerPhysicsComponent(Engine engine, Vector2 gameWorldPosition)
            : base(engine)
        {
            CreatePlayerPhysicsObjects(gameWorldPosition);
        }

        public void ResetPlayerPosition(Vector2 gameWorldPosition)
        {
            DestroyPlayerPhysicsObjects();
            CreatePlayerPhysicsObjects(gameWorldPosition);
        }

        private void CreatePlayerPhysicsObjects(Vector2 gameWorldPosition)
        {
            MainFixture = FixtureFactory.CreateRectangle(Engine.Physics.World, 0.5f, 0.5f, 1);
            Bodies.Add(MainFixture.Body);
            MainFixture.Body.Position = Engine.Physics.PositionToPhysicsWorld(gameWorldPosition);
            MainFixture.Body.BodyType = BodyType.Dynamic;
            MainFixture.Body.SleepingAllowed = false;

            MainFixture.CollisionFilter.CollisionCategories = (Category)(Global.CollisionCategories.Enemy);
            MainFixture.CollisionFilter.CollidesWith = (Category)(Global.CollisionCategories.Player | Global.CollisionCategories.PlayerBullet | Global.CollisionCategories.Structure | Global.CollisionCategories.Light);

            WheelFixture = FixtureFactory.CreateCircle(Engine.Physics.World, 0.3f, 1.0f);
            Bodies.Add(WheelFixture.Body);
            WheelFixture.Body.Position = MainFixture.Body.Position + new Vector2(0.0f, 0.6f);
            WheelFixture.Body.BodyType = BodyType.Dynamic;

            WheelFixture.Body.SleepingAllowed = false;
            WheelFixture.Friction = 0.5f;

            WheelFixture.CollisionFilter.CollisionCategories = (Category)(Global.CollisionCategories.Enemy);
            WheelFixture.CollisionFilter.CollidesWith = (Category)(Global.CollisionCategories.Player | Global.CollisionCategories.PlayerBullet | Global.CollisionCategories.Structure | Global.CollisionCategories.Light);

            playerFAJ = JointFactory.CreateFixedAngleJoint(Engine.Physics.World, MainFixture.Body);
            playerFAJ.BodyB = WheelFixture.Body;

            wheelMotorRevJoint = JointFactory.CreateRevoluteJoint(MainFixture.Body, WheelFixture.Body, Vector2.Zero);
            wheelMotorRevJoint.MaxMotorTorque = 10.0f;
            wheelMotorRevJoint.MotorEnabled = true;
            Engine.Physics.World.AddJoint(wheelMotorRevJoint);
        }

        private void DestroyPlayerPhysicsObjects()
        {
            Engine.Physics.World.RemoveJoint(playerFAJ);
            Engine.Physics.World.RemoveJoint(wheelMotorRevJoint);
            Engine.Physics.World.RemoveBody(WheelFixture.Body);
            Engine.Physics.World.RemoveBody(MainFixture.Body);
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
