using Microsoft.Xna.Framework;

using NePlus.Krypton;

namespace NePlus.Components.EngineComponents
{
    public class Lighting : Component
    {
        public KryptonEngine KryptonEngine;

        public Lighting(Engine engine) : base(engine)
        {
            KryptonEngine = new KryptonEngine(engine, @"Lighting\KryptonEffect");
            engine.AddComponent(this);

            //KryptonEngine.Matrix = Engine.Camera.CameraMatrix;
            KryptonEngine.Matrix = Matrix.CreateOrthographic(Engine.Video.GraphicsDevice.Viewport.Width / 10, Engine.Video.GraphicsDevice.Viewport.Height / 10, 0, 1);
            //KryptonEngine.Matrix = Matrix.CreateTranslation(Engine.Camera.Position.X / -Engine.Physics.PixelsPerMeter, Engine.Camera.Position.Y / -Engine.Physics.PixelsPerMeter, 0);
            //Vector2 size = Engine.Camera.CurSize / (Engine.Physics.PixelsPerMeter * 2.0f);
            //KryptonEngine.Matrix = Matrix.CreateOrthographicOffCenter(-size.X, size.X, size.Y, -size.Y, 0, 1);
            this.DrawOrder = int.MaxValue / 2;
        }

        public override void Initialize()
        {
            KryptonEngine.Initialize();

            base.Initialize();
        }

        public override void LoadContent()
        {
            KryptonEngine.LoadContent();

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            KryptonEngine.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            KryptonEngine.Draw(gameTime);

            base.Draw(gameTime);
        }

        public void DebugDraw()
        {
            // Clear the helpers vertices
            KryptonEngine.RenderHelper.Vertices.Clear();
            KryptonEngine.RenderHelper.Indicies.Clear();

            foreach (var hull in KryptonEngine.Hulls)
            {
                KryptonEngine.RenderHelper.BufferAddShadowHull(hull);
            }

            KryptonEngine.RenderHelper.Effect.CurrentTechnique = KryptonEngine.RenderHelper.Effect.Techniques["DebugDraw"];

            foreach (var effectPass in KryptonEngine.RenderHelper.Effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                KryptonEngine.RenderHelper.BufferDraw();
            }
        }
    }
}
