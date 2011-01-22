using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

using NePlus;
using NePlus.Components.GameComponents;
using NePlus.ScreenManagement;

namespace NePlus.GameObjects.LightObjects
{
    class NullLight : Light
    {
        List<Light> WorldLights;
        List<Light> AffectedLights;

        public NullLight(Engine engine, List<Light> worldLights, Vector2 position, float range, Color color, string motionType) : base(engine, position, range, color, motionType)
        {
            WorldLights = worldLights;
            AffectedLights = new List<Light>();
        }

        public override void ResolveLightEffect()
        {
            //foreach (Light light in WorldLights)
            //{
            //    if (light != this && !AffectedLights.Contains(light))
            //    {
            //        RotatedRectangle rectangle = new RotatedRectangle(new Rectangle((int)light.Position.X + (int)light.TextureOrigin.X, (int)light.Position.Y, light.Texture.Width, light.Texture.Height), PhysicsComponent.MainFixture.Body.Rotation);

            //        if (CollidingWithRectangle(rectangle))
            //        {
            //            AffectedLights.Add(light);
            //        }
            //    }
            //}

            //foreach (Light light in AffectedLights)
            //{
            //    // if the light isn't in the null light, EffectActive is set to true
            //    RotatedRectangle rectangle = new RotatedRectangle(new Rectangle((int)light.Position.X + (int)light.TextureOrigin.X, (int)light.Position.Y, light.Texture.Width, light.Texture.Height), PhysicsComponent.MainFixture.Body.Rotation);
            //    light.EffectActive = !CollidingWithRectangle(rectangle);
            //}
        }
    }
}
