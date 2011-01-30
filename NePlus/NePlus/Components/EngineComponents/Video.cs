using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using ProjectMercury.Renderers;

namespace NePlus.Components.EngineComponents
{
    public class Video : Component
    {
        // graphics device
        public GraphicsDevice GraphicsDevice { get; private set; }

        // particle effect renderer
        public Renderer ParticleRenderer { get; private set; }

        // resolution
        public int Height { get; private set; }
        public int Width { get; private set; }

        public Video(Engine engine) : base(engine)
        {
            // particle effects
            ParticleRenderer = new SpriteBatchRenderer
            {
                GraphicsDeviceService = Global.GraphicsDeviceManager
            };

            GraphicsDevice = Global.Game.GraphicsDevice;

            Engine.AddComponent(this);
        }

        public override void LoadContent()
        {
            ParticleRenderer.LoadContent(Global.Game.Content);
        }
    }
}