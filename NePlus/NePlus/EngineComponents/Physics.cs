using System;
using Microsoft.Xna.Framework;

using FarseerPhysics;
using FarseerPhysics.DebugViewXNA;
using FarseerPhysics.Dynamics;

namespace NePlus.EngineComponents
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Physics : Microsoft.Xna.Framework.GameComponent
    {
        // used for conversion between game and physics scaling
        public float PixelsPerMeter { get; private set; }

        // physics world
        public World World { get; private set; }

        // debug view
        public DebugViewXNA DebugView { get; private set; }
        
        public Physics(Game game)
            : base(game)
        {
            // this should probably never change
            PixelsPerMeter = 100.0f;

            World = new World(new Vector2(0.0f, 6.0f));

            DebugView = new DebugViewXNA(World);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            // physics
            World.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f, (1f / 30f)));

            base.Update(gameTime);
        }
        
        // conversion functions
        public Vector2 PositionToGameWorld(Vector2 position)
        {
            return position * PixelsPerMeter;
        }
        
        public Vector2 PositionToPhysicsWorld(Vector2 position)
        {
            return position / PixelsPerMeter;
        }

    }
}
