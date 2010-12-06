using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;

using NePlusEngine.Components.EngineComponents;

namespace NePlusEngine.Components.PhysicsComponents
{
    public class PhysicsComponent : Component
    {
        public Fixture MainFixture { get; protected set; }
        public Vector2 Position { get { return Engine.Physics.PositionToGameWorld(MainFixture.Body.Position); } }

        public PhysicsComponent(Engine engine) : base(engine) { }
    }
}
