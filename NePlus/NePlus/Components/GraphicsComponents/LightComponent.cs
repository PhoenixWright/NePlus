using Microsoft.Xna.Framework;

using Krypton;
using Krypton.Lights;

namespace NePlus.Components.GraphicsComponents
{
    public class LightComponent : Component
    {
        public Light2D Light;

        /// <summary>
        /// Constructs a light and adds it to the collection of game lights.
        /// </summary>
        /// <param name="engine">The game engine.</param>
        /// <param name="position">The light's position.</param>
        /// <param name="fov">The light's 'field of vision'.</param>
        /// <param name="angle">The light's angle in the game world.</param>
        /// <param name="range">The light's range.</param>
        /// <param name="color">The light's color.</param>
        public LightComponent(Engine engine, Vector2 position, float fov, float angle, float range, Color color)
            : base(engine)
        {
            Light = new Light2D();
            Light.Position = position;
            Light.Fov = fov;
            Light.Angle = angle;
            Light.Range = range;
            Light.Color = color;

            Light.Texture = LightTextureBuilder.CreateConicLight(Engine.Video.GraphicsDevice, 1024, MathHelper.TwoPi / 1);

            Engine.Lighting.Krypton.Lights.Add(Light);
        }
    }
}