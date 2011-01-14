using System;
using Microsoft.Xna.Framework;

using FarseerPhysics;
using FarseerPhysics.Collision;
using FarseerPhysics.Common;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics;

using NePlus.ScreenManagement;
using NePlus.ScreenManagement.Screens;

namespace NePlus.Components.EngineComponents
{
    public class Physics : Component
    {
        public Camera Camera { get; private set; }

        // used for conversion between game and physics scaling
        public float PixelsPerMeter { get; private set; }

        // physics world
        public World World { get; private set; }

        // debug view
        public DebugViewXNA DebugView { get; private set; }
        
        public Physics(Engine engine) : base(engine)
        {
            Camera = Engine.Camera;

            // this should probably never change
            PixelsPerMeter = Global.Configuration.GetFloatConfig("Physics", "PixelsPerMeter");

            World = new World(new Vector2(0.0f, 9.8f));

            DebugView = new DebugViewXNA(World);
            DebugView.LoadContent(Global.Game.GraphicsDevice, Engine.Content);
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
        }

        public override void Update(GameTime gameTime)
        {
            // update the physics world
            World.Step(Math.Min((float)gameTime.ElapsedGameTime.TotalMilliseconds * 0.001f, (1f / 30f)));
        }

        public override void Draw(GameTime gameTime)
        {
            if (Global.Configuration.GetBooleanConfig("Debug", "ShowDebugView"))
            {
                Matrix view = Matrix.CreateTranslation(Camera.Position.X / -PixelsPerMeter, Camera.Position.Y / -PixelsPerMeter, 0);
                Vector2 size = Camera.CurSize / (PixelsPerMeter * 2.0f);
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
            return position * Global.Configuration.GetFloatConfig("Physics", "PixelsPerMeter");
        }

        /// <summary>
        /// Conversion function to calculate the physics world position of a game world position.
        /// </summary>
        /// <param name="position">The game world position.</param>
        /// <returns>The physics world position.</returns>
        public Vector2 PositionToPhysicsWorld(Vector2 position)
        {
            return position / Global.Configuration.GetFloatConfig("Physics", "PixelsPerMeter");
        }

        /// <summary>
        /// Conversion function to calculate the game world value of a physics world value.
        /// </summary>
        /// <param name="value">The physics world value.</param>
        /// <returns>The game world value.</returns>
        public float ValueToGameWorld(float value)
        {
            return value * Global.Configuration.GetFloatConfig("Physics", "PixelsPerMeter");
        }

        /// <summary>
        /// Conversion function to calculate the physics world value of a game world value.
        /// </summary>
        /// <param name="value">The game world value.</param>
        /// <returns>The physics world value.</returns>
        public float ValueToPhysicsWorld(float value)
        {
            return value / Global.Configuration.GetFloatConfig("Physics", "PixelsPerMeter");
        }

        public AABB CreateAABB(float gameWorldWidth, float gameWorldHeight, Vector2 gameWorldPosition)
        {
            return new AABB(ValueToPhysicsWorld(gameWorldWidth), ValueToPhysicsWorld(gameWorldHeight), PositionToPhysicsWorld(gameWorldPosition));
        }
    }
}
