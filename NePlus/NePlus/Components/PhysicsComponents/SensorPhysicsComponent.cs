using System.Collections.Generic;

using Microsoft.Xna.Framework;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

namespace NePlus.Components.PhysicsComponents
{
    public class SensorPhysicsComponent : Component
    {
        public Fixture SensorFixture;

        public SensorPhysicsComponent(Engine engine, List<Vector2> vertices, Vector2 position)
            : base(engine)
        {
            Vertices farseerVertices = new Vertices(vertices);

            SensorFixture = FixtureFactory.CreatePolygon(Engine.Physics.World, farseerVertices, 0.0f);
            SensorFixture.Body.BodyType = BodyType.Static;
            SensorFixture.IsSensor = true;
        }
    }
}
