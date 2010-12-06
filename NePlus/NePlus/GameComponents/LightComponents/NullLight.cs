using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Dynamics;

using NePlusEngine;

namespace NePlus.GameComponents.LightComponents
{
    class NullLight : Light
    {
        List<Light> WorldLights;
        List<Light> AffectedLights;

        public NullLight(Engine engine, List<Light> worldLights, Vector2 position, string motionType) : base(engine, position, motionType)
        {
            WorldLights = worldLights;
            AffectedLights = new List<Light>();

            lightTextureName = "GreyTriangle";

            Texture = Engine.Content.Load<Texture2D>(lightTextureName);
            TextureOrigin = new Vector2(Texture.Width / 2, Texture.Height / 2);
        }

        public override void ResolveLightEffect()
        {
            foreach (Light light in WorldLights)
            {
                if (light != this)
                {
                    if (PositionInLight(light.Position + new Vector2(light.Texture.Width / 2, 0.0f)))
                    {
                        AffectedLights.Add(light);
                    }
                }
            }

            foreach (Light light in AffectedLights)
            {
                // if the light isn't in the null light, EffectActive is set to true
                light.EffectActive = !PositionInLight(light.Position + new Vector2(light.Texture.Width / 2, 0.0f));
            }
        }
    }
}
