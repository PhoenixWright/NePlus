using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

using FarseerPhysics;
using FarseerPhysics.DebugViewXNA;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using NePlus.Components;
using NePlus.Global;

namespace NePlus
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        public List<Fixture> Fixtures;

        // camera
        Camera camera;

        // input
        Input input;

        // farseer stuff
        World physicsWorld = new World(new Vector2(0.0f, 6.0f));
        DebugViewXNA debugView;
        public float PixelsPerMeter { get; private set; }
        
        // textures
        Texture2D platformTexture, boxTexture;

        // initial texture positions
        Vector2 platformPosition = Vector2.Zero;
        Vector2 boxPosition = Vector2.Zero;

        // farseer fixtures
        Fixture platformFixture, boxFixture;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";            
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // graphics
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            // camera
            //camera = new Camera(new Vector2(1280, 720));            

            // input
            input = new Input(this);
            input.Initialize();

            // farseer
            PixelsPerMeter = 100.0f;
            debugView = new DebugViewXNA(physicsWorld);

            // light code
            GravityLightComponent gravityLight = new GravityLightComponent(this);
            this.Components.Add(gravityLight);
            Fixtures = new List<Fixture>();

            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            
            InitializeDebugView();

            // setup box
            boxTexture = Content.Load<Texture2D>(@"TestContent\TestSquare");
            boxPosition.X = (1280 / 2) - (boxTexture.Width / 2);
            boxPosition.Y = 100;
            // farseer box stuff
            boxFixture = FixtureFactory.CreateRectangle(physicsWorld, boxTexture.Width / PixelsPerMeter, boxTexture.Height / PixelsPerMeter, 1);
            boxFixture.Body.Position = new Vector2(boxPosition.X / PixelsPerMeter, boxPosition.Y / PixelsPerMeter);
            boxFixture.Body.BodyType = BodyType.Dynamic;
            boxFixture.Restitution = 0.5f;

            Fixtures.Add(boxFixture);

            // set up platform
            platformTexture = Content.Load<Texture2D>(@"TestContent\TestRectangle");
            platformPosition.X = (1280 / 2) - (platformTexture.Width / 2);
            platformPosition.Y = 500;
            // farseer platform stuff
            platformFixture = FixtureFactory.CreateRectangle(physicsWorld, platformTexture.Width / PixelsPerMeter, platformTexture.Height / PixelsPerMeter, 1);
            platformFixture.Body.Position = new Vector2(platformPosition.X / PixelsPerMeter, platformPosition.Y / PixelsPerMeter);
            platformFixture.Body.BodyType = BodyType.Static;

            Fixtures.Add(platformFixture);

            //camera.TrackingBody = boxFixture.Body;
            //camera.Position = boxPosition;
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
            // TODO: Add your update logic here

            input.Update(gameTime);
            //camera.Update(input);

            // jump logic
            if (input.GetKeyStateFromAction(Enums.Action.JumpOrAccept) == Enums.KeyState.Pressed)
                boxFixture.Body.ApplyForce(new Vector2(0.0f, -10.0f));

            boxPosition.X = boxFixture.Body.Position.X * PixelsPerMeter;
            boxPosition.Y = boxFixture.Body.Position.Y * PixelsPerMeter;

            // update physics sim
            physicsWorld.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f, (1f / 30f)));
            
            // Allows the game to exit
            if (input.GetKeyStateFromAction(Enums.Action.Exit) == Enums.KeyState.Pressed)
                this.Exit();

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            // TODO: Add your drawing code here
            //spriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, camera.CameraMatrix);
            spriteBatch.Begin();
            spriteBatch.Draw(boxTexture, boxPosition, Color.White);
            spriteBatch.Draw(platformTexture, platformPosition, Color.White);
            spriteBatch.End();

            DrawDebugView();
            
            base.Draw(gameTime);
        }

        private void DrawDebugView()
        {
            // TODO: this code is broken

            //Matrix view = Matrix.CreateTranslation(camera.Position.X / -PixelsPerMeter, camera.Position.Y / -PixelsPerMeter, 0);
            //Vector2 size = camera.CurSize / (PixelsPerMeter * 2);
            //Matrix proj = Matrix.CreateOrthographicOffCenter(-size.X, size.X, size.Y, -size.Y, 0, 1);

            Matrix proj = Matrix.CreateOrthographic(PixelsPerMeter * GraphicsDevice.Viewport.AspectRatio, PixelsPerMeter, 0, 1);
            Matrix view = Matrix.Identity;
            debugView.RenderDebugData(ref proj, ref view);
        }

        private void InitializeDebugView()
        {
            DebugViewXNA.LoadContent(graphics.GraphicsDevice, Content);
            uint flags = 0;

            flags += (uint)DebugViewFlags.AABB;
            flags += (uint)DebugViewFlags.CenterOfMass;
            flags += (uint)DebugViewFlags.ContactNormals;
            flags += (uint)DebugViewFlags.ContactPoints;
            flags += (uint)DebugViewFlags.DebugPanel;
            flags += (uint)DebugViewFlags.Joint;
            flags += (uint)DebugViewFlags.Pair;
            flags += (uint)DebugViewFlags.PolygonPoints;
            flags += (uint)DebugViewFlags.Shape;
                
            debugView.Flags = (DebugViewFlags) flags;
        }
    }
}