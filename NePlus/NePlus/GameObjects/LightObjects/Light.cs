using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Dynamics;

using NePlus;
using NePlus.Components.EngineComponents;
using NePlus.Components.GameComponents;
using NePlus.Components.GraphicsComponents;
using NePlus.Components.PhysicsComponents;
using NePlus.ScreenManagement;
using NePlus.ScreenManagement.Screens;

namespace NePlus.GameObjects.LightObjects
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    abstract public class Light : Component
    {
        // configuration for the light
        public bool DrawLight { get; set; }
        public bool EffectActive { get; set; }

        public Vector2 Position { get; protected set; }
        public float Rotation { get; protected set; }
        public Func<Fixture, bool> EffectDelegate { get; protected set; }

        public LightingComponent LightingComponent;
        public PhysicsComponent PhysicsComponent;

        public Light(Engine engine, Vector2 position, float range, Color color, string motion)
            : base(engine)
        {
            DrawLight = true;
            EffectActive = true;

            Position = position;

            CreateLightComponent(position, range, color);
            CreatePhysicsComponent(motion);

            Engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            if (PhysicsComponent != null)
            {
                Position = PhysicsComponent.Position;
            }

            LightingComponent.Light.Position = Position;

            ResolveLightEffect();
        }

        private void CreateLightComponent(Vector2 position, float range, Color color)
        {
            LightingComponent = new LightingComponent(Engine, position, range, color);
        }

        private void CreatePhysicsComponent(string motionType)
        {
            switch (motionType)
            {
                case "None":
                    //PhysicsComponent = new RectanglePhysicsComponent(new Rectangle((int)Position.X, (int)Position.Y, 10, 10), Position, false);
                    break;
                case "Pendulum":                    
                    Point pivotPoint = new Point((int)Math.Round(Position.X), (int)Math.Round(Position.Y - 200));
                    Point weightPoint = new Point((int)Math.Round(Position.X + 200), (int)Math.Round(Position.Y));                    
                    PhysicsComponent = new PendulumPhysicsComponent(Engine, pivotPoint, weightPoint);
                    break;
                default:
                    throw new Exception("Physics component type not recognized");
            }
        }

        public bool CollidingWithRectangle(RotatedRectangle rectangle)
        {
            //RotatedRectangle myRectangle = new RotatedRectangle(new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height), PhysicsComponent.MainFixture.Body.Rotation);

            //return myRectangle.Intersects(rectangle);

            return false;
        }

        public bool PositionInLight(Vector2 position)
        {
            //Vector2 middleInGameWorld = Position + new Vector2(0.0f, Texture.Height / 2);

            //Engine.Physics.DebugView.DrawPoint(Engine.Physics.PositionToPhysicsWorld(middleInGameWorld), 0.1f, Color.Yellow);

            //bool positionInLight = position.X > middleInGameWorld.X - Texture.Width / 2
            //                    && position.X < middleInGameWorld.X + Texture.Width / 2
            //                    && position.Y > middleInGameWorld.Y - Texture.Height / 2
            //                    && position.Y < middleInGameWorld.Y + Texture.Height / 2;

            //return positionInLight;

            return false;
        }

        abstract public void ResolveLightEffect();
    }
}
