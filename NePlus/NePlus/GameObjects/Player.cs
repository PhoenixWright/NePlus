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
        // sprite batch
        SpriteBatch spriteBatch;

        // components
        public PhysicsComponent PhysicsComponent { get; private set; }

        // variables
        private Vector2 position;
        private Texture2D texture;

        public Player(Game game, Vector2 position) : base(game)
        {
            this.position = position;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            Game.Components.Add(this);
        }

        public override void Initialize()
        {
            this.LoadContent();
            PhysicsComponent = new PhysicsComponent(Game, texture.Bounds, position);

            base.Initialize();
        }

        protected override void LoadContent()
        {
            texture = Game.Content.Load<Texture2D>(@"TestContent\TestSquare");

            base.LoadContent();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            position = PhysicsComponent.Position;

            if (Engine.Input.IsCurPress(Engine.Configuration.JumpButton) || Engine.Input.IsCurPress(Engine.Configuration.JumpKey))
            {
                PhysicsComponent.Fixture.Body.ApplyForce(new Vector2(0.0f, -10.0f));
            }

            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Engine.Camera.CameraMatrix);
            spriteBatch.Draw(texture, position, null, Color.White, PhysicsComponent.Fixture.Body.Rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
