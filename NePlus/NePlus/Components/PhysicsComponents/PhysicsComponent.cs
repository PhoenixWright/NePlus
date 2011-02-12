using System.Collections.Generic;

using Microsoft.Xna.Framework;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

using NePlus.Components.EngineComponents;
using NePlus.ScreenManagement;
using NePlus.ScreenManagement.Screens;

namespace NePlus.Components.PhysicsComponents
{
    public class PhysicsComponent : Component
    {
        public Fixture MainFixture { get; protected set; }
        public float Angle { get { return MainFixture.Body.Rotation; } }
        public List<Body> Bodies { get; private set; }
        public Vector2 Position
        {
            get
            {
                return Engine.Physics.PositionToGameWorld(MainFixture.Body.Position);
            }
        }

        public PhysicsComponent(Engine engine) : base(engine)
        {
            Bodies = new List<Body>();

            Engine.AddComponent(this);
        }
    }
}