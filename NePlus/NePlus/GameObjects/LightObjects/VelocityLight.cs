using Microsoft.Xna.Framework;

using FarseerPhysics.Dynamics;

namespace NePlus.GameObjects.LightObjects
{
    public class VelocityLight : EffectLight
    {
        private float velocityValue { get; set; }
        private Vector2 velocityVectorX { get; set; }
        private Vector2 velocityVectorY { get; set; }

        public VelocityLight(Engine engine, float velocityValue)
            : base(engine)
        {
            this.velocityValue = velocityValue;
            velocityVectorX = new Vector2(velocityValue, 1.0f);
            velocityVectorY = new Vector2(1.0f, velocityValue);
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            if (EffectActive)
            {
                foreach (Fixture fixture in AffectedFixtures)
                {
                    if (velocityValue < 1.0f && velocityValue > -1.0f)
                    {
                        if (fixture.Body.LinearVelocity.X > 0.7f || fixture.Body.LinearVelocity.X < -0.7f)
                        {
                            fixture.Body.LinearVelocity *= velocityVectorX;
                        }

                        if (fixture.Body.LinearVelocity.Y > 0.7f || fixture.Body.LinearVelocity.Y < -0.7f)
                        {
                            fixture.Body.LinearVelocity *= velocityVectorY;
                        }
                    }
                    else
                    {
                        if (fixture.Body.LinearVelocity.X < 50.0f || fixture.Body.LinearVelocity.X > -50.0f)
                        {
                            fixture.Body.LinearVelocity *= velocityVectorX;
                        }

                        if (fixture.Body.LinearVelocity.Y < 50.0f || fixture.Body.LinearVelocity.Y > -50.0f)
                        {
                            fixture.Body.LinearVelocity *= velocityVectorY;
                        }
                    }
                }
            }

            base.Update(gameTime);
        }

        protected override void OnLightEntry(Fixture fixture)
        {
        }

        protected override void OnLightExit(Fixture fixture)
        {
        }
    }
}
