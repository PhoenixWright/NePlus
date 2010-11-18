using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using NePlus.EngineComponents;
using NePlus.GameComponents;

namespace NePlus.GameObjects
{
    public class Player : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // components
        public ParticleEffectComponent ParticleEffectComponent { get; private set; }
        public RectanglePhysicsComponent PhysicsComponent { get; private set; }

        // variables
        public Vector2 Position { get; private set; }
        private Texture2D texture;

        public Player(Game game, Vector2 position) : base(game)
        {
            Position = position;

            // need to load the texture before the PhysicsComponent
            this.LoadContent();
            //ParticleEffectComponent = new ParticleEffectComponent(game, "someName", Position);
            PhysicsComponent = new RectanglePhysicsComponent(texture.Bounds, position, true);

            Game.Components.Add(this);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>(@"TestSquare");

            base.LoadContent();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
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

            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            Engine.Video.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Engine.Camera.CameraMatrix);
            Engine.Video.SpriteBatch.Draw(texture, Position, null, Color.White, PhysicsComponent.MainFixture.Body.Rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            Engine.Video.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
