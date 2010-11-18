using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;

namespace NePlus.GameComponents
{
    public class PhysicsComponent
    {
        public Fixture MainFixture { get; protected set; }
        public Vector2 Position { get { return Engine.Physics.PositionToGameWorld(MainFixture.Body.Position); } }
    }
}
