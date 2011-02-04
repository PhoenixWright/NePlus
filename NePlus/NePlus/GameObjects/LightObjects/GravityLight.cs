using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;

using NePlus;
using NePlus.Components.EngineComponents;
using NePlus.Components.GraphicsComponents;
using NePlus.ScreenManagement;
using NePlus.ScreenManagement.Screens;

namespace NePlus.GameObjects.LightObjects
{
    public class GravityLight : Light
    {
        // components
        private ParticleEffectComponent particleEffectComponent;

        public float GravityValue { get; private set; }
        public Vector2 GravityVector { get; protected set; }

        public GravityLight(Engine engine, Vector2 position, float fov, float angle, float range, Color color, string motionType, float gravityValue) : base(engine, position, fov, angle, range, color, motionType)
        {
            particleEffectComponent = new ParticleEffectComponent(engine, "BeamMeUp", Position);

            GravityValue = gravityValue;
            GravityVector = new Vector2(0.0f, GravityValue);
        }

        public override void Update(GameTime gameTime)
        {
            particleEffectComponent.Position = Position - new Vector2(0.0f, -200.0f);
            particleEffectComponent.DrawParticleEffect = false;

            if (EffectActive)
            {
                foreach (Fixture fixture in AffectedFixtures)
                {
                    fixture.Body.ApplyForce(GravityVector);
                }
            }

            base.Update(gameTime);
        }
    }
}