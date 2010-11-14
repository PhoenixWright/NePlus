using Microsoft.Xna.Framework;

using ProjectMercury;
using ProjectMercury.Renderers;

namespace NePlus.GameComponents
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class ParticleEffectComponent : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private string particleEffectName;
        private ParticleEffect particleEffect;

        public ParticleEffectComponent(Game game, string effectName)
            : base(game)
        {
            particleEffectName = effectName;
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
            particleEffect = Game.Content.Load<ParticleEffect>(@"ParticleEffects\" + particleEffectName);
            particleEffect.LoadContent(Game.Content);
            particleEffect.Initialise();

            base.LoadContent();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            particleEffect.Trigger(new Vector2(Engine.Video.Width / 2, 0.0f));
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            particleEffect.Update(deltaSeconds);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Matrix cam = Engine.Camera.CameraMatrix;
            Engine.Video.ParticleRenderer.RenderEffect(particleEffect, ref cam);

            base.Draw(gameTime);
        }
    }
}
