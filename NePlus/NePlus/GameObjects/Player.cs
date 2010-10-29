using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NePlus.GameComponents;

namespace NePlus.GameObjects
{
    public class Player : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // components
        InputComponent inputComponent;
        PhysicsComponent physicsComponent;

        // variables
        Vector2 position;

        public Player(Game game) : base(game)
        {
            inputComponent = new InputComponent(game);            
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        public override void Update(Microsoft.Xna.Framework.GameTime gameTime)
        {
            physicsComponent.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(Microsoft.Xna.Framework.GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}
