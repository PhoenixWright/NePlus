using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

using NePlus.GameComponents;

using NePlusEngine;

namespace NePlus.GameComponents.LightComponents
{
    public class GravityLight : Light
    {
        // components
        private ParticleEffectComponent particleEffectComponent;

        public float GravityValue { get; private set; }

        private Vector2 gravityVector;
        public Vector2 GravityVector { get { return gravityVector; } }

        public GravityLight(Engine engine, Vector2 position, string motion, float gravityValue) : base(engine, position, motion)
        {
            particleEffectComponent = new ParticleEffectComponent(Engine, "BeamMeUp", Position);

            lightTextureName = "BlueTriangle";

            Texture = Engine.Content.Load<Texture2D>(lightTextureName);
            TextureOrigin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            GravityValue = gravityValue;
            gravityVector = new Vector2(0.0f, GravityValue);

            EffectDelegate = GravityEffect;
        }

        public override void Update()
        {
            particleEffectComponent.Position = Position + TextureOrigin;
            particleEffectComponent.DrawParticleEffect = EffectActive;

            base.Update();
        }
        
        public override void ResolveLightEffect()
        {
            if (EffectActive)
            {
                // create an AABB representing the light, and apply the gravity effect to anything in it
                AABB aabb = Engine.Physics.CreateAABB(Texture.Width, Texture.Height, Position);

                Engine.Physics.World.QueryAABB(EffectDelegate, ref aabb);
            }
        }

        private bool GravityEffect(Fixture fixture)
        {
            // check to make sure that the fixture is dynamic
            if (fixture.Body.BodyType == BodyType.Dynamic)
            {
                if (fixture.CollidesWith == CollisionCategory.Cat1)
                {
                    fixture.Body.ApplyForce(ref gravityVector);
                }
                
                return true;
            }

            return false;
        }
    }
}