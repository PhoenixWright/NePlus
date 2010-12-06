using Microsoft.Xna.Framework;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace NePlusEngine.Components.PhysicsComponents
{
    public class RectanglePhysicsComponent : PhysicsComponent
    {
        public RectanglePhysicsComponent(Engine engine, Rectangle rectangle, Vector2 gameWorldPosition, bool dynamic) : base(engine)
        {
            // create vertices to create a rectangle in the physics world with
            Vertices vertices = new Vertices();
            vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Left, rectangle.Top)));
            vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Right, rectangle.Top)));
            vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Right, rectangle.Bottom)));
            vertices.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rectangle.Left, rectangle.Bottom)));

            MainFixture = FixtureFactory.CreatePolygon(Engine.Physics.World, vertices, 1.0f);
            MainFixture.Body.Position = Engine.Physics.PositionToPhysicsWorld(gameWorldPosition);
            if (dynamic)
            {
                MainFixture.Body.BodyType = BodyType.Dynamic;
            }
            else
            {
                MainFixture.Body.BodyType = BodyType.Static;
            }
            MainFixture.Restitution = 0.5f;

            // adding some linear damping gives a max speed and seems to smooth out player motion really well
            MainFixture.Body.LinearDamping = 1.0f;
        }
    }
}
