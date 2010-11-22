using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Dynamics;

using NePlus.GameComponents.PhysicsComponents;
 
namespace NePlus.GameObjects
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    abstract public class Light : Microsoft.Xna.Framework.DrawableGameComponent
    {
        // configuration for the light
        public bool DrawLight { get; set; }
        public bool EffectActive { get; set; }

        protected string lightTextureName;

        public Vector2 Position { get; protected set; }
        public float Rotation { get; protected set; }
        public Texture2D Texture { get; private set; }
        public Vector2 TextureOrigin { get; private set; }
        public Func<Fixture, bool> EffectDelegate { get; protected set; }

        public PhysicsComponent PhysicsComponent;

        public Light(Game game, Vector2 position, string motion)
            : base(game)
        {
            DrawLight = true;
            EffectActive = true;

            Position = position;

            CreatePhysicsComponent(motion);
            
            Game.Components.Add(this);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            Texture = Game.Content.Load<Texture2D>(lightTextureName);
            TextureOrigin = new Vector2(Texture.Width / 2, Texture.Height / 2);

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            if (PhysicsComponent != null)
            {
                Position = PhysicsComponent.Position;
            }

            ResolveLightEffect();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
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

            base.Draw(gameTime);
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
                    PhysicsComponent = new PendulumPhysicsComponent(pivotPoint, weightPoint);
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
