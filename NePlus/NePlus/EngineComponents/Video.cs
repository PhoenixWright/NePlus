using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ProjectMercury.Renderers;

namespace NePlus.EngineComponents
{
    public class Video : Microsoft.Xna.Framework.GameComponent
    {
        // graphics device
        public GraphicsDevice GraphicsDevice { get; private set; }

        // graphics device manager
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        // spritebatch
        public SpriteBatch SpriteBatch { get; private set; }

        // particle effect renderer
        public Renderer ParticleRenderer { get; private set; }

        // resolution
        public int Height { get; private set; }
        public int Width { get; private set; }

        public Video(Game1 game) : base(game) { }

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

            // particle effects
            ParticleRenderer = new SpriteBatchRenderer
            {
                GraphicsDeviceService = Engine.Video.GraphicsDeviceManager
            };
        }

        public void LoadContent(Game1 game)
        {
            // grab the graphicsdevice from the Game class, which is first available during LoadContent which is why the code is here
            GraphicsDevice = game.GraphicsDevice;

            ParticleRenderer.LoadContent(Game.Content);

            // now that the GraphicsDevice exists we can create the game's SpriteBatch
            SpriteBatch = new SpriteBatch(GraphicsDevice);
        }
    }
}
