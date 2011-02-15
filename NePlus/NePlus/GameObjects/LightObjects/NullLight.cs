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

        public NullLight(Engine engine, Vector2 position, float fov, float angle, float range, Color color, string motionType, List<Light> worldLights) : base(engine, position, fov, angle, range, color, motionType)
        {
            WorldLights = worldLights;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (Light light in WorldLights)
            {
                if (light == this)
                {
                    continue;
                }

                if (PositionInLight(light.Position))
                {
                    if (AffectedLights.Contains(light))
                    {
                        continue;
                    }
                    else
                    {
                        AffectedLights.Add(light);
                    }
                }
                else
                {
                    if (AffectedLights.Contains(light))
                    {
                        light.EffectActive = true;
                        light.IsOn = true;
                        AffectedLights.Remove(light);
                    }
                }
            }

            if (EffectActive)
            {
                foreach (Light light in AffectedLights)
                {
                    light.EffectActive = false;
                    light.IsOn = false;
                }
            }

            base.Update(gameTime);
        }
    }
}
