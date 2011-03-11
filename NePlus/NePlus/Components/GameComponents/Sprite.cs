using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NePlus.Components.GameComponents
{
    public class Sprite : Component
    {
        private Texture2D sprite;
        private Vector2 spriteOrigin;

        public float Angle { get; set; }
        public Vector2 Position { get; set; }
        public SpriteEffects SpriteEffect { get; set; }
        public bool Visible { get; set; }

        public Sprite(Engine engine, string spriteFilepath)
            : base(engine)
        {
            sprite = engine.Content.Load<Texture2D>(spriteFilepath);
            spriteOrigin = new Vector2(sprite.Width / 2, sprite.Height / 2);

            Angle = 0.0f;
            Position = new Vector2();
            SpriteEffect = SpriteEffects.None;
            Visible = true;

            engine.AddComponent(this);
        }

        public override void Update(GameTime gameTime)
        {
        }

        public override void Draw(GameTime gameTime)
        {
            if (Visible)
            {
                Engine.SpriteBatch.Begin(SpriteSortMode.Immediate, null, null, null, null, null, Engine.Camera.CameraMatrix);
                Engine.SpriteBatch.Draw(sprite, Position, sprite.Bounds, Color.White, Angle, spriteOrigin, 1.0f, SpriteEffect, 1.0f);
                Engine.SpriteBatch.End();
            }
        }

        public override void Dispose(bool disposing)
        {
            sprite = null;

            base.Dispose(disposing);
        }
    }
}
