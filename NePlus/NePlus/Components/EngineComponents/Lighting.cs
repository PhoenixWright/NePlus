using System;

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

            // draw a Farseer shape representing the light's actual area of effect
            foreach (PointLight light in Krypton.Lights)
            {
                // if the angle isn't zero, then it's a cone-shaped light
                if (light.Angle != 0)
                {
                    // the first vector is the light's position
                    Vector2 position = Engine.Physics.PositionToPhysicsWorld(light.Position);

                    // the second vector is the first endpoint, which should take into account angle and range, where angle takes into account where the light is aimed
                    Vector2 endpoint1 = Engine.Physics.PositionToPhysicsWorld(light.Position + new Vector2(light.Range / 2.5f, light.Range / 2.5f));
                    endpoint1 = XnaHelper.RotateVector2(endpoint1, light.Angle - MathHelper.PiOver2 + 0.17f, position);
                    
                    // the third vector is the second endpoint, which should take into account angle, range, and the light's "fov", or the light's interior angle
                    Vector2 endpoint2 = XnaHelper.RotateVector2(endpoint1, light.Fov, position);

                    Vector2[] vertices = new Vector2[3];
                    vertices[0] = position;
                    vertices[1] = endpoint1;
                    vertices[2] = endpoint2;
                    Engine.Physics.DebugView.DrawPolygon(vertices, 3, Color.White);
                }
            }
        }
    }
}