using Microsoft.Xna.Framework;

using NePlus.Krypton;

namespace NePlus.Components.EngineComponents
{
    public class Lighting : Component
    {
        KryptonEngine kryptonEngine;

        public Lighting(Engine engine) : base(engine)
        {
            kryptonEngine = new KryptonEngine(engine, @"Lighting\KryptonEffect");
            engine.AddComponent(this);

            kryptonEngine.AmbientColor = new Color(35, 35, 35);
            kryptonEngine.Matrix = Engine.Camera.CameraMatrix;
        }

        public override void Initialize()
        {
            kryptonEngine.Initialize();

            base.Initialize();
        }

        public override void LoadContent()
        {
            kryptonEngine.LoadContent();

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            kryptonEngine.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //kryptonEngine.LightMapPrepare();

            //kryptonEngine.Draw(gameTime);

            //DebugDraw();

            base.Draw(gameTime);
        }

        private void DebugDraw()
        {
            // Clear the helpers vertices
            kryptonEngine.RenderHelper.Vertices.Clear();
            kryptonEngine.RenderHelper.Indicies.Clear();

            foreach (var hull in kryptonEngine.Hulls)
            {
                kryptonEngine.RenderHelper.BufferAddShadowHull(hull);
            }

            kryptonEngine.RenderHelper.Effect.CurrentTechnique = kryptonEngine.RenderHelper.Effect.Techniques["DebugDraw"];

            foreach (var effectPass in kryptonEngine.RenderHelper.Effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                kryptonEngine.RenderHelper.BufferDraw();
            }
        }
    }
}
