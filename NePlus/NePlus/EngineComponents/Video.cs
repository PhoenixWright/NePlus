using Microsoft.Xna.Framework;

namespace NePlus.EngineComponents
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Video : Microsoft.Xna.Framework.GameComponent
    {
        // resolution
        public int Height { get; private set; }
        public int Width { get; private set; }

        public Video(Game game)
            : base(game)
        {
            // resolution
            Height = 720;
            Width = 1280;
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }
    }
}