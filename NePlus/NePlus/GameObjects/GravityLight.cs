using Microsoft.Xna.Framework;

using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;

using NePlus.GameComponents;

namespace NePlus.GameObjects
{
    public class GravityLight : Light
    {
        public float GravityValue { get; private set; }

        private Vector2 gravityVector;
        public Vector2 GravityVector { get { return gravityVector; } }

        public GravityLight(Vector2 position, float gravityValue) : base(Engine.Game, position)
        {
            GravityValue = gravityValue;
            gravityVector = new Vector2(0.0f, GravityValue);

            EffectDelegate = GravityEffect;
        }

        public override void ResolveLightEffect()
        {
            // create an AABB representing the light, and apply the gravity effect to anything in it
            AABB aabb = Engine.Physics.CreateAABB(LightTexture.Width, LightTexture.Height, LightPosition);

            Engine.Physics.World.QueryAABB(EffectDelegate, ref aabb);
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