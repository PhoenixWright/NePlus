using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Dynamics;

using NePlusEngine;
using NePlusEngine.Components.PhysicsComponents;
 
namespace NePlus.GameComponents.LightComponents
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    abstract public class Light : Component
    {
        // configuration for the light
        public bool DrawLight { get; set; }
        public bool EffectActive { get; set; }

        protected string lightTextureName;

        public Vector2 Position { get; protected set; }
        public float Rotation { get; protected set; }
        public Texture2D Texture { get; protected set; }
        public Vector2 TextureOrigin { get; protected set; }
        public Func<Fixture, bool> EffectDelegate { get; protected set; }

        public PhysicsComponent PhysicsComponent;

        public Light(Engine engine, Vector2 position, string motion)
            : base(engine)
        {
            DrawLight = true;
            EffectActive = true;

            Position = position;

            CreatePhysicsComponent(motion);

            Engine.AddComponent(this);
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update()
        {
            if (PhysicsComponent != null)
            {
                Position = PhysicsComponent.Position;
            }

            ResolveLightEffect();
        }

        public override void Draw()
        {
            if (DrawLight)
            {
                Engine.Video.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Engine.Camera.CameraMatrix);
                
                if (PhysicsComponent != null)
                {
                    Engine.Video.SpriteBatch.Draw(Texture, Position, null, Color.White, PhysicsComponent.MainFixture.Body.Rotation, Vector2.Zero, 1.0f, SpriteEffects.None, 1.0f);
                }
                else
                {
                    Engine.Video.SpriteBatch.Draw(Texture, Position, Color.White);
                }

                Engine.Video.SpriteBatch.End();
            }
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

        public bool PositionInLight(Vector2 position)
        {
            Vector2 originInGameWorld = Position + TextureOrigin;

            bool positionInLight = position.X > originInGameWorld.X - Texture.Width / 2
                                && position.X < originInGameWorld.X + Texture.Width / 2
                                && position.Y > originInGameWorld.Y - Texture.Height / 2
                                && position.Y < originInGameWorld.Y + Texture.Height / 2;

            return positionInLight;
        }

        abstract public void ResolveLightEffect();
    }
}
