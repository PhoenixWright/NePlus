using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Krypton;
using Krypton.Lights;

namespace NePlus.Components.EngineComponents
{
    public class Lighting : Component
    {
        public KryptonEngine Krypton;

        public Lighting(Engine engine) : base(engine)
        {
            Krypton = new KryptonEngine(engine, @"Krypton\KryptonEffect");
            engine.AddComponent(this);

            Krypton.Bluriness = 3;
            Krypton.CullMode = CullMode.None;
            Krypton.Matrix = Engine.Camera.CameraMatrix;
            Krypton.SpriteBatchCompatablityEnabled = true;

            this.DrawOrder = int.MaxValue / 2;
        }

        public override void Initialize()
        {
            Krypton.Initialize();

            base.Initialize();
        }

        public override void LoadContent()
        {
            Krypton.LoadContent();

            base.LoadContent();
        }

        public override void Update(GameTime gameTime)
        {
            Krypton.Matrix = Engine.Camera.CameraMatrix;
            Krypton.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            Krypton.Draw(gameTime);

            base.Draw(gameTime);
        }

        public void DebugDraw()
        {
            // Clear the helpers vertices
            Krypton.RenderHelper.ShadowHullVertices.Clear();
            Krypton.RenderHelper.ShadowHullIndicies.Clear();

            foreach (var hull in Krypton.Hulls)
            {
                Krypton.RenderHelper.BufferAddShadowHull(hull);
            }

            Krypton.RenderHelper.Effect.CurrentTechnique = Krypton.RenderHelper.Effect.Techniques["DebugDraw"];

            foreach (var effectPass in Krypton.RenderHelper.Effect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                Krypton.RenderHelper.BufferDraw();
            }
        }
    }
}