using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using NePlus.GameComponents;

using NePlus;
using NePlus.Components.EffectComponents;
using NePlus.Components.PhysicsComponents;

namespace NePlus.GameObjects
{
    public class Player : Component
    {
        // components
        public ParticleEffectComponent ParticleEffectComponent { get; private set; }
        public RectanglePhysicsComponent PhysicsComponent { get; private set; }

        // variables
        public Vector2 Position { get; private set; }
        private Texture2D texture;

        public Player(Engine engine, Vector2 position) : base(engine)
        {
            Position = position;

            texture = Engine.Content.Load<Texture2D>(@"Characters\TestSquare");

            //ParticleEffectComponent = new ParticleEffectComponent(engine, "someName", Position);
            PhysicsComponent = new RectanglePhysicsComponent(engine, texture.Bounds, position, true);
            PhysicsComponent.MainFixture.CollisionFilter.CollidesWith = FarseerPhysics.Dynamics.Category.Cat1;

            Engine.AddComponent(this);
        }

        public override void Update()
        {
            Position = PhysicsComponent.Position;

            if (Engine.Input.IsCurPress(Engine.Configuration.JumpButton) || Engine.Input.IsCurPress(Engine.Configuration.JumpKey))
            {
                // using world center as the point to apply force to; this makes the point of the force application the center of the fixture
                PhysicsComponent.MainFixture.Body.ApplyForce(new Vector2(0.0f, -6.0f), PhysicsComponent.MainFixture.Body.WorldCenter);
            }

            if (Engine.Input.IsCurPress(Engine.Configuration.LeftButton) || Engine.Input.IsCurPress(Engine.Configuration.LeftKey))
            {
                PhysicsComponent.MainFixture.Body.ApplyForce(new Vector2(-2.0f, 0.0f), PhysicsComponent.MainFixture.Body.WorldCenter);
            }

            if (Engine.Input.IsCurPress(Engine.Configuration.RightButton) || Engine.Input.IsCurPress(Engine.Configuration.RightKey))
            {
                PhysicsComponent.MainFixture.Body.ApplyForce(new Vector2(2.0f, 0.0f), PhysicsComponent.MainFixture.Body.WorldCenter);
            }

            //ParticleEffectComponent.Position = this.Position;
        }

        public override void Draw()
        {
            Engine.Video.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Engine.Camera.CameraMatrix);
            Engine.Video.SpriteBatch.Draw(texture, Position, null, Color.White, PhysicsComponent.MainFixture.Body.Rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            Engine.Video.SpriteBatch.End();
        }
    }
}
