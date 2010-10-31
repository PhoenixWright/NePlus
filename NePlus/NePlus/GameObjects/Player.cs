using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using NePlus.GameComponents;

namespace NePlus.GameObjects
{
    public class Player : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // components
        private InputComponent inputComponent;
        public PhysicsComponent PhysicsComponent { get; private set; }

        // variables
        private Vector2 position;
        private Texture2D texture;

        public Player(Game game, Vector2 position) : base(game)
        {
            this.position = position;
            
            // the physics component needs to know the texture rectangle
            this.LoadContent();

            inputComponent = new InputComponent(game);
            PhysicsComponent = new PhysicsComponent(game, texture.Bounds, position);
            
            // add the component to the game
            game.Components.Add(this);
        }

        public override void Initialize()
        {
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
            

            base.Draw(gameTime);
        }
    }
}
