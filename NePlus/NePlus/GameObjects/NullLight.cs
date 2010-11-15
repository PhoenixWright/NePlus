using System.Collections.Generic;

using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;

namespace NePlus.GameObjects
{
    class NullLight : Light
    {
        public NullLight(Vector2 position) : base(Engine.Game, position)
        {
            lightTextureName = "GreyTriangle";

            OriginalEffectDelegate = NullEffect;
            CurrentEffectDelegate = NullEffect;
        }

        public override void ResolveLightEffect()
        {
            foreach (Light light in Engine.Level.Lights)
            {
                if (light != this)
                {
                    if (PositionInLight(light.LightPosition + new Vector2(light.CurrentLightTexture.Width / 2, -light.CurrentLightTexture.Height)))
                    {
                        light.CurrentEffectDelegate = this.CurrentEffectDelegate;
                        light.CurrentLightTexture = this.CurrentLightTexture;
                    }
                    else
                    {
                        light.CurrentEffectDelegate = light.OriginalEffectDelegate;
                        light.CurrentLightTexture = light.OriginalLightTexture;
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            if (Engine.Input.IsCurPress(Engine.Configuration.DebugDownButton) || Engine.Input.IsCurPress(Engine.Configuration.DebugDownKey))
            {
                LightPosition += new Vector2(0.0f, 10.0f);
            }

            if (Engine.Input.IsCurPress(Engine.Configuration.DebugUpButton) || Engine.Input.IsCurPress(Engine.Configuration.DebugUpKey))
            {
                LightPosition += new Vector2(0.0f, -10.0f);
            }

            if (Engine.Input.IsCurPress(Engine.Configuration.DebugLeftButton) || Engine.Input.IsCurPress(Engine.Configuration.DebugLeftKey))
            {
                LightPosition += new Vector2(-10.0f, 0.0f);
            }

            if (Engine.Input.IsCurPress(Engine.Configuration.DebugRightButton) || Engine.Input.IsCurPress(Engine.Configuration.DebugRightKey))
            {
                LightPosition += new Vector2(10.0f, 0.0f);
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
