using Microsoft.Xna.Framework;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

using NePlus.GameComponents;

namespace NePlus.GameObjects
{
    public class GravityLight : Light
    {
        // components
        private ParticleEffectComponent particleEffectComponent;

        public float GravityValue { get; private set; }

        private Vector2 gravityVector;
        public Vector2 GravityVector { get { return gravityVector; } }

        public GravityLight(Vector2 position, string motion, float gravityValue) : base(Engine.Game, position, motion)
        {
            particleEffectComponent = new ParticleEffectComponent(Engine.Game, "BeamMeUp", Position);

            lightTextureName = "BlueTriangle";

            GravityValue = gravityValue;
            gravityVector = new Vector2(0.0f, GravityValue);

            EffectDelegate = GravityEffect;
        }

        public override void Update(GameTime gameTime)
        {
            particleEffectComponent.Position = Position + TextureOrigin;
            particleEffectComponent.DrawParticleEffect = EffectActive;

            base.Update(gameTime);
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