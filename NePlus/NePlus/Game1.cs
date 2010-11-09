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
            Engine.Level = new EngineComponents.Level(this, @"Maps\TestMap");
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            Engine.LoadContent();
            
            player = new Player(this, Engine.Level.GetSpawnPoint());

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

            // allows the game to exit
            if (Engine.Input.IsCurPress(Engine.Configuration.QuitButton) || Engine.Input.IsCurPress(Engine.Configuration.QuitKey))
                this.Exit();

            // grab gravity light position from level
            Vector2 lightPosition = Engine.Level.Lights[0].LightPosition;
            Texture2D lightTexture = Engine.Level.Lights[0].LightTexture;

            // check if the light is hitting the player
            if (player.Position.X > lightPosition.X - lightTexture.Width / 2
                && player.Position.X < lightPosition.X + lightTexture.Width / 2 
                && player.Position.Y > lightPosition.Y - lightTexture.Height / 2
                && player.Position.Y < lightPosition.Y + lightTexture.Height / 2)
            {
                player.PhysicsComponent.Fixture.Body.ApplyForce(new Vector2(0.0f, -8.0f));
            }

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

            Engine.Video.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Engine.Camera.CameraMatrix);
            Engine.Video.SpriteBatch.End();

            // particles
            Matrix cam = Engine.Camera.CameraMatrix;
            particleRenderer.RenderEffect(particleEffect, ref cam);
            
            base.Draw(gameTime);
        }
    }
}