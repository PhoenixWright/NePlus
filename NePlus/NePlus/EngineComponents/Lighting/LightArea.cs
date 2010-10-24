using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NePlus.EngineComponents.Lighting
{
    class LightArea
    {
        private GraphicsDevice graphicsDevice;

        public RenderTarget2D RenderTarget { get; private set; }
        public Vector2 LightPosition { get; set; }
        public Vector2 LightAreaSize { get; set; }

        public LightArea(GraphicsDevice graphicsDevice, ShadowMapSize size)
        {
            int baseSize = 2 << (int)size;
            LightAreaSize = new Vector2(baseSize);
            RenderTarget = new RenderTarget2D(graphicsDevice, baseSize, baseSize);
            this.graphicsDevice = graphicsDevice;
        }

        public Vector2 ToRelativePosition(Vector2 worldPosition)
        {
            return worldPosition - (LightPosition - LightAreaSize * 0.5f);
        }

        public void BeginDrawingShadowCasters()
        {
            graphicsDevice.SetRenderTarget(RenderTarget);
            graphicsDevice.Clear(Color.Transparent);
        }

        public void EndDrawingShadowCasters()
        {
            graphicsDevice.SetRenderTarget(null);
        }
    }
}