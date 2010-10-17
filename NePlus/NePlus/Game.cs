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

        // input
        InputComponent input;

        // farseer stuff
        World world = new World(new Vector2(0, -20));
        DebugViewXNA debugView;
        private Fixture rectFix;
        private Fixture circFix;

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
            // initialization
            input = new InputComponent(this);
            this.Components.Add(input);
            debugView = new DebugViewXNA(world);

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

            DebugViewXNA.LoadContent(graphics.GraphicsDevice, Content);
            uint flags = 0;

            //flags += (uint)DebugViewFlags.AABB;
            //flags += (uint)DebugViewFlags.CenterOfMass;
            //flags += (uint)DebugViewFlags.ContactNormals;
            //flags += (uint)DebugViewFlags.ContactPoints;
            //flags += (uint)DebugViewFlags.DebugPanel;
            //flags += (uint)DebugViewFlags.Joint;
            //flags += (uint)DebugViewFlags.Pair;
            //flags += (uint)DebugViewFlags.PolygonPoints; 
            flags += (uint)DebugViewFlags.Shape;
                
            debugView.Flags = (DebugViewFlags) flags;

            rectFix = FixtureFactory.CreateRectangle(world, 50, 5, 1, Vector2.Zero);
            rectFix.Body.IsStatic = true;
            rectFix.Restitution = 0.3f;
            rectFix.Friction = 0.5f;

            circFix = FixtureFactory.CreateCircle(world, 2, 1, new Vector2(10, 10));
            circFix.Body.BodyType = BodyType.Dynamic;
            circFix.Restitution = 0.3f;
            circFix.Friction = 0.5f;

            // TODO: use this.Content to load your game content here
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
            // Allows the game to exit
            if (input.GetKeyStateFromAction(GameActions.Action.Exit) == InputComponent.KeyState.Down)
                this.Exit();

            // TODO: Add your update logic here
            // update physics sim
            world.Step((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f);

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
            
            // farseer stuff
            Matrix proj = Matrix.CreateOrthographic(50 * graphics.GraphicsDevice.Viewport.AspectRatio, 50, 0, 1);
            Matrix view = Matrix.Identity;

            debugView.RenderDebugData(ref proj, ref view);
            
            base.Draw(gameTime);
        }
    }
}
