using Microsoft.Xna.Framework;

namespace NePlus.GameObjects
{
    class NullLight : Light
    {
        public NullLight(Vector2 position) : base(Engine.Game, position)
        {
        }

        public override void ResolveLightEffect()
        {
            throw new System.NotImplementedException();
        }
    }
}
