using Microsoft.Xna.Framework;

using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;

using NePlus.EngineComponents;

namespace NePlus.GameComponents
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class PhysicsComponent : Microsoft.Xna.Framework.GameComponent
    {
        private Physics physics;

        public Vector2 Position
        {
            get { return physics.PositionToGameWorld(Fixture.Body.Position); }
        }

        public Fixture Fixture { get; private set; }

        public PhysicsComponent(Game game, Rectangle rectangle, Vector2 gameWorldPosition) : base(game)
        {
            physics = game.Engine.Physics;
            
            // create vertices to create a rectangle in Farseer with
            Vertices vertices = new Vertices();
            vertices.Add(physics.PositionToPhysicsWorld(new Vector2(rectangle.Left, rectangle.Top)));
            vertices.Add(physics.PositionToPhysicsWorld(new Vector2(rectangle.Right, rectangle.Top)));
            vertices.Add(physics.PositionToPhysicsWorld(new Vector2(rectangle.Right, rectangle.Bottom)));
            vertices.Add(physics.PositionToPhysicsWorld(new Vector2(rectangle.Left, rectangle.Bottom)));

            Fixture = FixtureFactory.CreatePolygon(physics.World, vertices, 1.0f);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {

            base.Update(gameTime);
        }
    }
}
