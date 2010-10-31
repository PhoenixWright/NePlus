using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using NePlus.GameComponents;

namespace NePlus.GameObjects
{
    public class Player : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // sprite batch
        SpriteBatch spriteBatch;

        // components
        private InputComponent inputComponent;
        public PhysicsComponent PhysicsComponent { get; private set; }

        // variables
        private Vector2 position;
        private Texture2D texture;

        public Player(Game1 game, Vector2 position) : base(game)
        {
            this.position = position;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);

            Game.Components.Add(this);
        }

        public override void Initialize()
        {
            inputComponent = new InputComponent(Game);

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

            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Game1.Engine.Camera.CameraMatrix);
            spriteBatch.Draw(texture, position, Color.White);
            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
