using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using FarseerPhysics.Dynamics;

namespace NePlus.GameObjects
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    abstract public class Light : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected string lightTextureName;

        public Vector2 LightOrigin { get; private set; }
        public Vector2 LightPosition { get; protected set; }
        public Texture2D OriginalLightTexture { get; private set; }
        public Texture2D CurrentLightTexture { get; set; }

        public Func<Fixture, bool> OriginalEffectDelegate { get; protected set; }
        public Func<Fixture, bool> CurrentEffectDelegate { get; set; }

        public Light(Game game, Vector2 position)
            : base(game)
        {
            LightPosition = position;
            
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
            OriginalLightTexture = Game.Content.Load<Texture2D>(lightTextureName);
            LightOrigin = new Vector2(OriginalLightTexture.Width / 2, 0.0f);
            CurrentLightTexture = OriginalLightTexture;

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            ResolveLightEffect();

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Engine.Video.SpriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, null, null, null, null, Engine.Camera.CameraMatrix);
            //Engine.Video.SpriteBatch.Draw(CurrentLightTexture, LightPosition, null, Color.White, 0.0f, LightOrigin, 1.0f, SpriteEffects.None, 0.0f);
            Engine.Video.SpriteBatch.Draw(CurrentLightTexture, LightPosition, Color.White);
            //Engine.Physics.DebugView.DrawPoint(Engine.Physics.PositionToPhysicsWorld(LightOrigin + LightPosition), 1.0f, Color.Green);
            Engine.Video.SpriteBatch.End();

            base.Draw(gameTime);
        }

        public bool PositionInLight(Vector2 position)
        {
            bool positionInLight = position.X > LightPosition.X
              && position.X < LightPosition.X + CurrentLightTexture.Width
              && position.Y > LightPosition.Y - CurrentLightTexture.Height
              && position.Y < LightPosition.Y;

            return positionInLight;
        }

        abstract public void ResolveLightEffect();
    }
}
