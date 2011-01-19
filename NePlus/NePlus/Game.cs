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
        GraphicsDeviceManager graphics;
        ScreenManager screenManager;

        public NePlusGame()
        {
            
            
            // tried to move this code, but it seems that nothing will draw unless it is located here
            // I seriously can't believe how many times this has come up
            graphics = new GraphicsDeviceManager(this);
            Global.Initialize(this, graphics);
            graphics.PreferredBackBufferWidth = Global.Configuration.GetIntConfig("Video", "Width");
            graphics.PreferredBackBufferHeight = Global.Configuration.GetIntConfig("Video", "Height");

            screenManager = new ScreenManager(this);
            Components.Add(screenManager);

            screenManager.AddScreen(new BackgroundScreen(), null);
            screenManager.AddScreen(new MainMenuScreen(), null);
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
            graphics.GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);
        }
    }
}