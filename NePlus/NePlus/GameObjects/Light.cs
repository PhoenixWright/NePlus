using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NePlus.GameObjects
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Light : Microsoft.Xna.Framework.DrawableGameComponent
    {
        public Texture2D LightTexture { get; private set; }
        public Vector2 LightPosition { get; private set; }

        public Light(Game game, Vector2 position)
            : base(game)
        {
            LightPosition = position;
            
            Game.Components.Add(this);       
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            LightTexture = Game.Content.Load<Texture2D>("BlueTriangle");

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.Video.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Engine.Camera.CameraMatrix);
            Engine.Video.SpriteBatch.Draw(LightTexture, LightPosition, null, Color.White, 0.0f, new Vector2(LightTexture.Bounds.Center.X, LightTexture.Bounds.Center.Y), 1.0f, SpriteEffects.None, 0.0f);
            Engine.Video.SpriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
