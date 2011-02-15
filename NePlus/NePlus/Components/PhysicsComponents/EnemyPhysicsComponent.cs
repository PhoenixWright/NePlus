using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace NePlus.Components.PhysicsComponents
{
    public class EnemyPhysicsComponent : PhysicsComponent
    {
        public EnemyPhysicsComponent(Engine engine, Vector2 gameWorldPosition)
            : base(engine)
        {
            MainFixture = FixtureFactory.CreateRectangle(Engine.Physics.World, 0.5f, 0.5f, 1);
            Bodies.Add(MainFixture.Body);
            MainFixture.Body.Position = Engine.Physics.PositionToPhysicsWorld(gameWorldPosition);
            MainFixture.Body.BodyType = BodyType.Dynamic;
        }
    }
}
