using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Dynamics;

using NePlusEngine;
using NePlusEngine.Components.GameComponents;

namespace NePlus.GameComponents.LightComponents
{
    class NullLight : Light
    {
        List<Light> WorldLights;
        List<Light> AffectedLights;

        public NullLight(Engine engine, List<Light> worldLights, Vector2 position, string motionType) : base(engine, @"Lights\GreySpotlight", position, motionType)
        {
            WorldLights = worldLights;
            AffectedLights = new List<Light>();

            DrawOrder = int.MaxValue - 1;
        }

        public override void ResolveLightEffect()
        {
            foreach (Light light in WorldLights)
            {
                if (light != this && !AffectedLights.Contains(light))
                {
                    RotatedRectangle rectangle = new RotatedRectangle(new Rectangle((int)light.Position.X + (int)light.TextureOrigin.X, (int)light.Position.Y, light.Texture.Width, light.Texture.Height), PhysicsComponent.MainFixture.Body.Rotation);

                    //if (PositionInLight(light.Position + light.TextureOrigin))
                    if (CollidingWithRectangle(rectangle))
                    {
                        AffectedLights.Add(light);
                    }
                }
            }

            foreach (Light light in AffectedLights)
            {
                // if the light isn't in the null light, EffectActive is set to true
                //light.EffectActive = !PositionInLight(light.Position + light.TextureOrigin);
                RotatedRectangle rectangle = new RotatedRectangle(new Rectangle((int)light.Position.X + (int)light.TextureOrigin.X, (int)light.Position.Y, light.Texture.Width, light.Texture.Height), PhysicsComponent.MainFixture.Body.Rotation);
                light.EffectActive = !CollidingWithRectangle(rectangle);
            }
        }
    }
}
