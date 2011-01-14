using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using NePlus.Components.EngineComponents;

namespace NePlus
{
    public static class Global
    {
        public static Configuration Configuration { get; private set; }
        public static ContentManager Content { get; private set; }
        public static Game Game { get; private set; }
        public static GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

        public static void Initialize(Game game, GraphicsDeviceManager gdm)
        {
            Configuration = new Configuration();
            Content = game.Content;
            Content.RootDirectory = "Content";
            Game = game;
            GraphicsDeviceManager = gdm;
        }
    }
}
