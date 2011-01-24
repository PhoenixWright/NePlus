using Microsoft.Xna.Framework;

using NePlus.Krypton;
using NePlus.Krypton.Lights;

namespace NePlus.Components.GraphicsComponents
{
    public class LightingComponent : Component
    {
        public PointLight Light;

        public LightingComponent(Engine engine, Vector2 position, float range, Color color)
            : base(engine)
        {
            Light = new PointLight();
            Light.Position = position;
            Light.Range = range + 20;
            Light.Color = color;

            Light.Texture = LightTextureBuilder.CreateConicLight(Engine.Video.GraphicsDevice, 1024, MathHelper.TwoPi / 1);

            Engine.Lighting.KryptonEngine.Lights.Add(Light);
        }
    }
}
