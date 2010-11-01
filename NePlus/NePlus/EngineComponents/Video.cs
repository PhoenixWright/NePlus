using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NePlus.EngineComponents
{
    public class Video
    {
        // graphics device
        public GraphicsDevice GraphicsDevice { get; private set; }

        // graphics device manager
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        // spritebatch
        public SpriteBatch SpriteBatch { get; private set; }

        // resolution
        public int Height { get; private set; }
        public int Width { get; private set; }

        public void Initialize(Game1 game)
        {
            // resolution
            Height = 720;
            Width = 1280;

            // TODO: create this stuff somewhere other than Game1
            GraphicsDeviceManager = game.graphics;

            // graphics device manager
            GraphicsDeviceManager.PreferredBackBufferWidth = Width;
            GraphicsDeviceManager.PreferredBackBufferHeight = Height;
            GraphicsDeviceManager.ApplyChanges();
        }

        public void LoadContent(Game1 game)
        {
            // grab the graphicsdevice from the Game class, which is first available during LoadContent which is why the code is here
            GraphicsDevice = game.GraphicsDevice;

            // now that the GraphicsDevice exists we can create the game's SpriteBatch
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }
    }
}
