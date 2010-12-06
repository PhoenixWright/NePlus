using System;
using Microsoft.Xna.Framework;

using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.DebugViewXNA;
using FarseerPhysics.Dynamics;

namespace NePlusEngine.EngineComponents
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Physics : Component
    {
        // used for conversion between game and physics scaling
        public float PixelsPerMeter { get; private set; }

        // physics world
        public World World { get; private set; }

        // debug view
        public DebugViewXNA DebugView { get; private set; }
        
        public Physics(Engine engine) : base(engine)
        {
            // this should probably never change
            PixelsPerMeter = 100.0f;

            World = new World(new Vector2(0.0f, 9.8f));

            DebugView = new DebugViewXNA(World);

            // TODO: make this a little more dynamic as far as options go
            DebugViewXNA.LoadContent(Engine.Game.GraphicsDevice, Engine.Content);
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
            DebugView.Flags = (DebugViewFlags)flags;
            
            DrawOrder = int.MaxValue;

            Engine.AddComponent(this);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update()
        {
            // update the physics world
            World.Step(Math.Min((float)Engine.GameTime.ElapsedGameTime.TotalMilliseconds * 0.001f, (1f / 30f)));
        }

        public override void Draw()
        {
            if (Engine.Configuration.ShowDebugView)
            {
                Matrix view = Matrix.CreateTranslation(Engine.Camera.Position.X / -PixelsPerMeter, Engine.Camera.Position.Y / -PixelsPerMeter, 0);
                Vector2 size = Engine.Camera.CurSize / (PixelsPerMeter * 2.0f);
                Matrix proj = Matrix.CreateOrthographicOffCenter(-size.X, size.X, size.Y, -size.Y, 0, 1);

                DebugView.DrawSegment(new Vector2(-25, 0), new Vector2(25, 0), Color.Red);
                DebugView.DrawSegment(new Vector2(0, -25), new Vector2(0, 25), Color.Green);
                DebugView.RenderDebugData(ref proj, ref view);
            }
        }
        
        /// <summary>
        /// Conversion function to calculate the game world position of a Farseer physics position.
        /// </summary>
        /// <param name="position">The physics world position.</param>
        /// <returns>The game world position.</returns>
        public Vector2 PositionToGameWorld(Vector2 position)
        {
            return position * PixelsPerMeter;
        }
        
        /// <summary>
        /// Conversion function to calculate the physics world position of a game world position.
        /// </summary>
        /// <param name="position">The game world position.</param>
        /// <returns>The physics world position.</returns>
        public Vector2 PositionToPhysicsWorld(Vector2 position)
        {
            return position / PixelsPerMeter;
        }

        /// <summary>
        /// Conversion function to calculate the game world value of a physics world value.
        /// </summary>
        /// <param name="value">The physics world value.</param>
        /// <returns>The game world value.</returns>
        public float ValueToGameWorld(float value)
        {
            return value * PixelsPerMeter;
        }

        /// <summary>
        /// Conversion function to calculate the physics world value of a game world value.
        /// </summary>
        /// <param name="value">The game world value.</param>
        /// <returns>The physics world value.</returns>
        public float ValueToPhysicsWorld(float value)
        {
            return value / PixelsPerMeter;
        }

        public AABB CreateAABB(float gameWorldWidth, float gameWorldHeight, Vector2 gameWorldPosition)
        {
            return new AABB(ValueToPhysicsWorld(gameWorldWidth), ValueToPhysicsWorld(gameWorldHeight), PositionToPhysicsWorld(gameWorldPosition));
        }
    }
}