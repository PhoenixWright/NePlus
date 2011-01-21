using Microsoft.Xna.Framework;

using ProjectMercury;
using ProjectMercury.Renderers;

using NePlus;
using NePlus.ScreenManagement;
using NePlus.ScreenManagement.Screens;

namespace NePlus.Components.GraphicsComponents
{
    public class ParticleEffectComponent : Component
    {
        public bool DrawParticleEffect { get; set; }

        private ParticleEffect particleEffect;
        private string particleEffectName;
        public Vector2 Position { get; set; }

        public ParticleEffectComponent(Engine engine, string effectName, Vector2 initialPosition)
            : base(engine)
        {
            DrawParticleEffect = true;
            particleEffectName = effectName;
            this.Position = initialPosition;
            
            particleEffect = Engine.Content.Load<ParticleEffect>(@"ParticleEffects\" + particleEffectName);
            particleEffect.LoadContent(Engine.Content);
            particleEffect.Initialise();

            DrawOrder = int.MaxValue - 1;

            Engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
            particleEffect.Trigger(Position);
            float deltaSeconds = (float)gameTime.ElapsedGameTime.TotalSeconds;
            particleEffect.Update(deltaSeconds);
        }

        public override void Draw(GameTime gameTime)
        {
            if (DrawParticleEffect)
            {
                Matrix cam = Engine.Camera.CameraMatrix;
                Engine.Video.ParticleRenderer.RenderEffect(particleEffect, ref cam);
            }
        }
    }
}