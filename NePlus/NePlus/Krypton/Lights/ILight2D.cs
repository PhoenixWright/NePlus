using System.Collections.Generic;

namespace NePlus.Krypton.Lights
{
    public interface ILight2D
    {
        bool IsOn { get; set; }

        void Draw(KryptonRenderHelper helper);
        void DrawShadows(KryptonRenderHelper helper, List<ShadowHull> hullNode);
    }
}