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

using NePlus.GameObjects;

namespace NePlus
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public GraphicsDeviceManager graphics;

        // textures
        Texture2D platformTexture;

        // initial texture positions
        Vector2 platformPosition = Vector2.Zero;
        Vector2 boxPosition = Vector2.Zero;

        // farseer fixtures
        Fixture platformFixture;

        // particle effects
        ParticleEffect particleEffect;
        Renderer particleRenderer;

        // player
        Player player;

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
            Engine.Initialize(this);
            
            player = new Player(this, new Vector2(720, 0));
            
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Engine.LoadContent();

            // set up platform
            platformTexture = Content.Load<Texture2D>(@"TestContent\TestRectangle");
            platformPosition.X = (Engine.Video.Width / 2);
            platformPosition.Y = 500;

            Rectangle rect2 = platformTexture.Bounds;
            Vertices verts2 = new Vertices();
            verts2.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rect2.Left, rect2.Top)));
            verts2.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rect2.Right, rect2.Top)));
            verts2.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rect2.Right, rect2.Bottom)));
            verts2.Add(Engine.Physics.PositionToPhysicsWorld(new Vector2(rect2.Left, rect2.Bottom)));
            
            // farseer platform stuff
            platformFixture = FixtureFactory.CreatePolygon(Engine.Physics.World, verts2, 1.0f);
            platformFixture.Body.Position = Engine.Physics.PositionToPhysicsWorld(new Vector2(platformPosition.X, platformPosition.Y));
            platformFixture.Body.BodyType = BodyType.Static;

            // particles
            particleEffect = Content.Load<ParticleEffect>(@"ParticleEffects\Rain");
            particleEffect.LoadContent(Content);
            particleEffect.Initialise();

            particleRenderer = new SpriteBatchRenderer
            {
                GraphicsDeviceService = Engine.Video.GraphicsDeviceManager
            };
            particleRenderer.LoadContent(Content);

            Engine.Camera.Position = player.PhysicsComponent.Fixture.Body.Position;
            Engine.Camera.TrackingBody = player.PhysicsComponent.Fixture.Body;
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
            Engine.Update(gameTime);
            
            // Allows the game to exit
            if (Engine.Input.IsCurPress(Engine.Configuration.QuitButton) || Engine.Input.IsCurPress(Engine.Configuration.QuitKey))
                this.Exit();

            // particles
            particleEffect.Trigger(new Vector2(Engine.Video.Width / 2, 0.0f));
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            particleEffect.Update(deltaSeconds);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Engine.Video.GraphicsDevice.Clear(Color.Black);

            Engine.Video.SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Engine.Camera.CameraMatrix);
            Engine.Video.SpriteBatch.Draw(platformTexture, platformPosition, Color.White);
            Engine.Video.SpriteBatch.End();

            // particles
            Matrix cam = Engine.Camera.CameraMatrix;
            particleRenderer.RenderEffect(particleEffect, ref cam);
            
            base.Draw(gameTime);
        }
    }
}