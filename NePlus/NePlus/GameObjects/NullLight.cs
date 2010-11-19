using System.Collections.Generic;

using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;

namespace NePlus.GameObjects
{
    class NullLight : Light
    {
        HashSet<Light> AffectedLights;

        public NullLight(Vector2 position, string motionType) : base(Engine.Game, position, motionType)
        {
            AffectedLights = new HashSet<Light>();

            lightTextureName = "GreyTriangle";

            EffectDelegate = NullEffect;
        }

        public override void ResolveLightEffect()
        {
            foreach (Light light in Engine.Level.Lights)
            {
                if (light != this)
                {
                    if (PositionInLight(light.Position + new Vector2(light.Texture.Width / 2, -light.Texture.Height)))
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
            if (Engine.Input.IsCurPress(Engine.Configuration.DebugDownButton) || Engine.Input.IsCurPress(Engine.Configuration.DebugDownKey))
            {
                Position += new Vector2(0.0f, 10.0f);
            }

            if (Engine.Input.IsCurPress(Engine.Configuration.DebugUpButton) || Engine.Input.IsCurPress(Engine.Configuration.DebugUpKey))
            {
                Position += new Vector2(0.0f, -10.0f);
            }

            if (Engine.Input.IsCurPress(Engine.Configuration.DebugLeftButton) || Engine.Input.IsCurPress(Engine.Configuration.DebugLeftKey))
            {
                Position += new Vector2(-10.0f, 0.0f);
            }

            if (Engine.Input.IsCurPress(Engine.Configuration.DebugRightButton) || Engine.Input.IsCurPress(Engine.Configuration.DebugRightKey))
            {
                Position += new Vector2(10.0f, 0.0f);
            }

            base.Update(gameTime);
        }

        private bool NullEffect(Fixture fixture)
        {
            ResolveLightEffect();

            return false;
        }
    }
}
