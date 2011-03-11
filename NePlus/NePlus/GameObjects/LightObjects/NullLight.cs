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
    class NullLight : EffectLight
    {
        List<EffectLight> WorldLights;

        public NullLight(Engine engine, List<EffectLight> worldLights)
            : base(engine)
        {
            WorldLights = worldLights;
        }

        public override void Update(GameTime gameTime)
        {
            foreach (EffectLight light in WorldLights)
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
                foreach (EffectLight light in AffectedLights)
                {
                    light.EffectActive = false;
                    light.IsOn = false;
                }
            }

            base.Update(gameTime);
        }
    }
}
