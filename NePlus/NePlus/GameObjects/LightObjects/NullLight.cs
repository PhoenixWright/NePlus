using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using NePlus;
using NePlus.Components.GameComponents;
using NePlus.ScreenManagement;

namespace NePlus.GameObjects.LightObjects
{
    class NullLight : Light
    {
        List<Light> WorldLights;
        List<Light> AffectedLights;

        public NullLight(Engine engine, Vector2 position, float fov, float angle, float range, Color color, string motionType, List<Light> worldLights) : base(engine, position, fov, angle, range, color, motionType)
        {
            WorldLights = worldLights;
            AffectedLights = new List<Light>();
        }
    }
}