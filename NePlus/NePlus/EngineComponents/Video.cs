using Microsoft.Xna.Framework;

namespace NePlus.EngineComponents
{
    public class Video
    {
        // graphics device manager
        public GraphicsDeviceManager Graphics { get; private set; }

        // resolution
        public int Height { get; private set; }
        public int Width { get; private set; }

        public void Initialize()
        {
            // resolution
            Height = 720;
            Width = 1280;

            // graphics device manager
            Graphics = new GraphicsDeviceManager(Engine.Game);
            Graphics.PreferredBackBufferWidth = Width;
            Graphics.PreferredBackBufferHeight = Height;
            Graphics.ApplyChanges();
        }
    }
}
