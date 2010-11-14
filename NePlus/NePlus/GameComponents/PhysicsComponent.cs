using Microsoft.Xna.Framework;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using NePlus.EngineComponents;

namespace NePlus.GameComponents
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PhysicsComponent : Microsoft.Xna.Framework.GameComponent
    {
        private Physics physics;

        public Vector2 Position { get { return physics.PositionToGameWorld(Fixture.Body.Position); } }

        public Fixture Fixture { get; private set; }

        public  PhysicsComponent(Game game, Rectangle rectangle, Vector2 gameWorldPosition, bool dynamic) : base(game)
        {
            physics = Engine.Physics;

            // create vertices to create a rectangle in the physics world with
            Vertices vertices = new Vertices();
            vertices.Add(physics.PositionToPhysicsWorld(new Vector2(rectangle.Left, rectangle.Top)));
            vertices.Add(physics.PositionToPhysicsWorld(new Vector2(rectangle.Right, rectangle.Top)));
            vertices.Add(physics.PositionToPhysicsWorld(new Vector2(rectangle.Right, rectangle.Bottom)));
            vertices.Add(physics.PositionToPhysicsWorld(new Vector2(rectangle.Left, rectangle.Bottom)));

            Fixture = FixtureFactory.CreatePolygon(physics.World, vertices, 1.0f);
            Fixture.Body.Position = physics.PositionToPhysicsWorld(gameWorldPosition);
            if (dynamic)
            {
                Fixture.Body.BodyType = BodyType.Dynamic;
            }
            else
            {
                Fixture.Body.BodyType = BodyType.Static;
            }
            Fixture.Restitution = 0.5f;

            // adding some linear damping gives a max speed and seems to smooth out player motion really well
            Fixture.Body.LinearDamping = 1.0f;
        }
    }
}
