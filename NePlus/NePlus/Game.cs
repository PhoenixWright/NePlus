using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.DebugViewXNA;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using NePlus.EngineComponents.Lighting;

namespace NePlus
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        Engine Engine;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;       
        
        // textures
        Texture2D platformTexture, boxTexture;

        // initial texture positions
        Vector2 platformPosition = Vector2.Zero;
        Vector2 boxPosition = Vector2.Zero;

        // farseer fixtures
        Fixture platformFixture, boxFixture;

        // lighting
        Vector2 lightPosition;
        QuadRenderComponent quadRenderer;
        ShadowMapResolver shadowMapResolver;
        LightArea lightArea;
        RenderTarget2D screenShadows;

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
            Engine = new Engine(this);

            // graphics
            graphics.PreferredBackBufferWidth = Engine.Video.Width;
            graphics.PreferredBackBufferHeight = Engine.Video.Height;
            graphics.ApplyChanges();        
            
            // farseer
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
            Engine.Physics.DebugView.Flags = (DebugViewFlags)flags;

            // lighting
            quadRenderer = new QuadRenderComponent(this);
            this.Components.Add(quadRenderer);

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

            // setup box
            boxTexture = Content.Load<Texture2D>(@"TestContent\TestSquare");
            boxPosition.X = 720;
            boxPosition.Y = 0;

            // farseer box stuff
            boxFixture = FixtureFactory.CreateRectangle(Engine.Physics.World, boxTexture.Width / Engine.Physics.PixelsPerMeter, boxTexture.Height / Engine.Physics.PixelsPerMeter, 1.0f);
            boxFixture.Body.Position = Engine.Physics.PositionToPhysicsWorld(boxPosition);
            boxFixture.Body.BodyType = BodyType.Dynamic;
            boxFixture.Restitution = 0.5f;

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

            // lighting
            shadowMapResolver = new ShadowMapResolver(GraphicsDevice, quadRenderer, ShadowMapSize.Size256, ShadowMapSize.Size1024);
            shadowMapResolver.LoadContent(Content);
            lightArea = new LightArea(GraphicsDevice, ShadowMapSize.Size512);
            lightPosition = boxPosition;
            screenShadows = new RenderTarget2D(GraphicsDevice, GraphicsDevice.Viewport.Width, GraphicsDevice.Viewport.Height);

            Engine.Camera.Position = boxFixture.Body.Position;
            Engine.Camera.TrackingBody = boxFixture.Body;
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

            // jump logic
            if (Engine.Input.IsCurPress(Buttons.A))
                boxFixture.Body.ApplyForce(new Vector2(0.0f, -10.0f));
            
            // Allows the game to exit
            if (Engine.Input.IsCurPress(Buttons.Back))
                this.Exit();
            
            boxPosition = Engine.Physics.PositionToGameWorld(boxFixture.Body.Position);
            lightPosition = boxPosition;

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            Vector2 origin = new Vector2(boxTexture.Width / 2, boxTexture.Height / 2);
            // lighting
            lightArea.LightPosition = lightPosition;
            lightArea.BeginDrawingShadowCasters();
            spriteBatch.Begin();            
            spriteBatch.Draw(platformTexture, lightArea.ToRelativePosition(platformPosition), Color.Black);
            spriteBatch.End();
            lightArea.EndDrawingShadowCasters();
            shadowMapResolver.ResolveShadows(lightArea.RenderTarget, lightArea.RenderTarget, lightPosition);

            GraphicsDevice.SetRenderTarget(screenShadows);
            GraphicsDevice.Clear(Color.Black);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.Additive);
            spriteBatch.Draw(lightArea.RenderTarget, lightArea.LightPosition - lightArea.LightAreaSize * 0.5f, Color.Red);
            spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Engine.Camera.CameraMatrix);
            spriteBatch.Draw(boxTexture, boxPosition, null, Color.White, 0.0f, origin, 1.0f, SpriteEffects.None, 0.0f);
            spriteBatch.Draw(platformTexture, platformPosition, Color.White);
            spriteBatch.End();

            BlendState blendState = new BlendState();
            blendState.ColorSourceBlend = Blend.DestinationColor;
            blendState.ColorDestinationBlend = Blend.SourceColor;

            spriteBatch.Begin(SpriteSortMode.Immediate, blendState, null, null, null, null, Engine.Camera.CameraMatrix);
            spriteBatch.Draw(screenShadows, Vector2.Zero, Color.White);
            spriteBatch.End();

            Matrix view = Matrix.CreateTranslation(Engine.Camera.Position.X / -Engine.Physics.PixelsPerMeter, Engine.Camera.Position.Y / -Engine.Physics.PixelsPerMeter, 0);
            Vector2 size = Engine.Camera.CurSize / (Engine.Physics.PixelsPerMeter * 2.0f);
            Matrix proj = Matrix.CreateOrthographicOffCenter(-size.X, size.X, size.Y, -size.Y, 0, 1);

            Engine.Physics.DebugView.DrawSegment(new Vector2(-25, 0), new Vector2(25, 0), Color.Red);
            Engine.Physics.DebugView.DrawSegment(new Vector2(0, -25), new Vector2(0, 25), Color.Green);
            Engine.Physics.DebugView.RenderDebugData(ref proj, ref view);
            
            base.Draw(gameTime);
        }
    }
}