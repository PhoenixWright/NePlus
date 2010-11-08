using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

using NePlus.EngineComponents;

namespace NePlus
{
    public static class Engine
    {
        // game reference
        public static Game1 Game { get; private set; }

        // content reference
        public static ContentManager Content { get; private set; }

        public static Camera Camera { get; private set; }
        public static Configuration Configuration { get; private set; }
        public static Input Input { get; private set; }
        public static Level Level { get; private set; }
        public static Physics Physics { get; private set; }
        public static Video Video { get; private set; }
        
        public static void Initialize(Game1 game)
        {
            Game = game;
            
            Content = game.Content;
            Content.RootDirectory = "Content";

            Video = new Video();
            Video.Initialize(Game);
            
            Camera = new Camera(new Vector2(Video.Width, Video.Height));
            Configuration = new Configuration();
            Input = new Input();
            Level = new Level(Game);
            Physics = new Physics(Game);
        }

        public static void LoadContent()
        {
            Video.LoadContent(Game);
        }

        public static void Update(GameTime gameTime)
        {
            Input.Update();
            Camera.Update();
        }
    }
}
