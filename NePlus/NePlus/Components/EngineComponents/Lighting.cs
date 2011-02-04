﻿using System;

using Microsoft.Xna.Framework;

using NePlus.Krypton;
using NePlus.Krypton.Lights;

namespace NePlus.Components.EngineComponents
{
    public class Lighting : Component
    {
        public KryptonEngine Krypton;

        public Lighting(Engine engine) : base(engine)
        {
            Krypton = new KryptonEngine(engine, @"Krypton\KryptonEffect");
            Krypton.AmbientColor = new Color(65, 65, 65);
            engine.AddComponent(this);

            Krypton.BlurEnable = true;
            Krypton.BlurFactorU = 1.0f / (Engine.Video.GraphicsDevice.Viewport.Width / 10);
            Krypton.BlurFactorV = 1.0f / (Engine.Video.GraphicsDevice.Viewport.Height / 10);
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