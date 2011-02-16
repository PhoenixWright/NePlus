using System;

using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;
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
