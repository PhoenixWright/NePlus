using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.DebugViewXNA;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using ProjectMercury;
using ProjectMercury.Emitters;
using ProjectMercury.Modifiers;
using ProjectMercury.Renderers;

using TiledLib;

using NePlusEngine;

using NePlus.GameComponents;
using NePlus.GameObjects;

namespace NePlus
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;

        // engine
        NePlusEngine.Engine engine;

        // player
        Player player;

        // level
        Level level;

        public Game1()
        {
            // tried to move this code, but it seems that nothing will draw unless it is located here
            graphics = new GraphicsDeviceManager(this);
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            engine = new NePlusEngine.Engine(this, graphics);
            level = new Level(engine, @"Maps\TestMap");

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            engine.LoadContent(this);
            
            player = new Player(engine, level.GetSpawnPoint());

            engine.Camera.Position = player.PhysicsComponent.MainFixture.Body.Position;
            engine.Camera.TrackingBody = player.PhysicsComponent.MainFixture.Body;
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // update the engine
            engine.Update(gameTime);            

            // allows the game to exit
            if (engine.Input.IsCurPress(engine.Configuration.QuitButton) || engine.Input.IsCurPress(engine.Configuration.QuitKey))
                this.Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            engine.Video.GraphicsDevice.Clear(Color.Black);

            engine.Draw();

            // this code is placeholder code for testing
            engine.Video.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, engine.Camera.CameraMatrix);
            engine.Video.SpriteBatch.End();
            
            base.Draw(gameTime);
        }
    }
}