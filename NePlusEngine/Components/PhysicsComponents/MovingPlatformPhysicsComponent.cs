using System.Collections.Generic;

using Microsoft.Xna.Framework;

using NePlusEngine;
using NePlusEngine.Components.PhysicsComponents;

namespace NePlus.GameComponents.PhysicsComponents
{
    public class MovingPlatformPhysicsComponent : PhysicsComponent
    {
        public List<Vector2> Positions { get; private set; }

        public MovingPlatformPhysicsComponent(Engine engine, List<Vector2> gameWorldPositionList, float speed)
            : base(engine)
        {
            Engine.AddComponent(this);
        }

        public override void Update()
        {
        }
    }
}
