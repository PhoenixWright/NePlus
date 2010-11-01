using Microsoft.Xna.Framework;

using NePlus.EngineComponents;

namespace NePlus
{
    public static class Engine
    {
        // game reference
        public static Game1 Game { get; private set; }

        public static Camera Camera { get; private set; }
        public static Configuration Configuration { get; private set; }
        public static Input Input { get; private set; }
        public static Physics Physics { get; private set; }
        public static Video Video { get; private set; }
        
        public static void Initialize(Game1 game)
        {
            Game = game;

            Video = new Video();
            Video.Initialize();
            
            Camera = new Camera(new Vector2(Video.Width, Video.Height));
            Configuration = new Configuration();
            Input = new Input();
            Physics = new Physics(Game);
        }

        public static void Update(GameTime gameTime)
        {
            Input.Update();
            Camera.Update();
        }
    }
}
