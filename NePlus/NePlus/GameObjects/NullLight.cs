using System.Collections.Generic;

using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;

namespace NePlus.GameObjects
{
    class NullLight : Light
    {
        List<Light> AffectedLights;

        public NullLight(Vector2 position, string motionType) : base(Engine.Game, position, motionType)
        {
            AffectedLights = new List<Light>();

            lightTextureName = "GreyTriangle";
        }

        public override void ResolveLightEffect()
        {
            foreach (Light light in Engine.Level.Lights)
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

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}
