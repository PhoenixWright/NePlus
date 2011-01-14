using Microsoft.Xna.Framework;

using NePlus.ScreenManagement;
using NePlus.ScreenManagement.Screens;

namespace NePlus
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class NePlusGame : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager GraphicsDeviceManager;
        ScreenManager screenManager;

        public NePlusGame()
        {
            // tried to move this code, but it seems that nothing will draw unless it is located here
            // I seriously can't believe how many times this has come up
            GraphicsDeviceManager = new GraphicsDeviceManager(this);
            
            Global.Initialize(this, GraphicsDeviceManager);
           
            GraphicsDeviceManager.PreferredBackBufferWidth = Global.Configuration.GetIntConfig("Video", "Width");
            GraphicsDeviceManager.PreferredBackBufferHeight = Global.Configuration.GetIntConfig("Video", "Height");

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            screenManager.AddScreen(new BackgroundScreen());
            screenManager.AddScreen(new MainMenuScreen());
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            base.LoadContent();
        }

        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}