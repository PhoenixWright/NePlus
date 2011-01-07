using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

using NePlusEngine;
using NePlusEngine.Components.EffectComponents;

namespace NePlus.GameObjects.LightObjects
{
    public class GravityLight : Light
    {
        // components
        private ParticleEffectComponent particleEffectComponent;

        public float GravityValue { get; private set; }

        private Vector2 gravityVector;
        public Vector2 GravityVector { get { return gravityVector; } }

        public GravityLight(Engine engine, Vector2 position, string motion, float gravityValue) : base(engine, @"Lights\BlueSpotlight", position, motion)
        {
            particleEffectComponent = new ParticleEffectComponent(Engine, "BeamMeUp", Position);

            GravityValue = gravityValue;
            gravityVector = new Vector2(0.0f, GravityValue);

            EffectDelegate = GravityEffect;
        }

        public override void Update()
        {
            particleEffectComponent.Position = Position - new Vector2(0.0f, -200.0f);
            particleEffectComponent.DrawParticleEffect = EffectActive;

            base.Update();
        }
        
        public override void ResolveLightEffect()
        {
            if (EffectActive)
            {
                // create an AABB representing the light, and apply the gravity effect to anything in it
                AABB aabb = Engine.Physics.CreateAABB(Texture.Width, Texture.Height, Position + new Vector2(-Texture.Width / 2, 0.0f));

                Engine.Physics.DebugView.DrawAABB(ref aabb, Color.Yellow);

                Engine.Physics.World.QueryAABB(EffectDelegate, ref aabb);
            }
        }

        private bool GravityEffect(Fixture fixture)
        {
            // check to make sure that the fixture is dynamic
            if (fixture.Body.BodyType == BodyType.Dynamic)
            {
                if (fixture.CollisionFilter.CollidesWith == Category.Cat1)
                {
                    fixture.Body.ApplyForce(ref gravityVector);

                    Engine.Physics.DebugView.DrawPoint(fixture.Body.Position, 0.05f, Color.Yellow);
                }
                
                return true;
            }

            return false;
        }
    }
}