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

        public GravityLight(Vector2 position, float gravityValue) : base(Engine.Game, position)
        {
            particleEffectComponent = new ParticleEffectComponent(Engine.Game, "BeamMeUp", LightPosition);

            lightTextureName = "BlueTriangle";

            GravityValue = gravityValue;
            gravityVector = new Vector2(0.0f, GravityValue);

            OriginalEffectDelegate = GravityEffect;
            CurrentEffectDelegate = GravityEffect;
        }

        public override void Update(GameTime gameTime)
        {
            particleEffectComponent.Position = LightPosition + LightOrigin;

            base.Update(gameTime);
        }

        public override void ResolveLightEffect()
        {
            // create an AABB representing the light, and apply the gravity effect to anything in it
            AABB aabb = Engine.Physics.CreateAABB(CurrentLightTexture.Width, CurrentLightTexture.Height, LightPosition);

            Engine.Physics.World.QueryAABB(CurrentEffectDelegate, ref aabb);
        }

        private bool GravityEffect(Fixture fixture)
        {
            // check to make sure that the fixture is dynamic
            if (fixture.Body.BodyType == BodyType.Dynamic)
            {
                 fixture.Body.ApplyForce(ref gravityVector);

                return true;
            }

            return false;
        }
    }
}