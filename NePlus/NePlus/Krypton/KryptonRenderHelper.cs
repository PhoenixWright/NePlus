using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using NePlus.Krypton.Lights;

namespace NePlus.Krypton
{
    public enum BlendTechnique
    {
        Add = 1,
        Multiply = 2,
    };

    public enum BlurTechnique
    {
        Horizontal = 1,
        Vertical = 2,
    };

    public class KryptonRenderHelper
    {
        #region Static Unit Quad

        private static VertexPositionTexture[] UnitQuad = new VertexPositionTexture[]
        {
            new VertexPositionTexture()
            {
                Position = new Vector3(-1, 1, 0),
                TextureCoordinate = new Vector2(0, 0),
            },
            new VertexPositionTexture()
            {
                Position = new Vector3(1, 1, 0),
                TextureCoordinate = new Vector2(1, 0),
            },
            new VertexPositionTexture()
            {
                Position = new Vector3(-1, -1, 0),
                TextureCoordinate = new Vector2(0, 1),
            },
            new VertexPositionTexture()
            {
                Position = new Vector3(1, -1, 0),
                TextureCoordinate = new Vector2(1, 1),
            },
        };

        #endregion Static Unit Quad

        private GraphicsDevice mGraphicsDevice;
        private Effect mEffect;
        private List<VertexPositionNormalTexture> mVertices = new List<VertexPositionNormalTexture>();
        private List<Int32> mIndicies = new List<Int32>();

        public GraphicsDevice GraphicsDevice
        {
            get { return this.mGraphicsDevice; }
        }
        public Effect Effect
        {
            get { return this.mEffect; }
        }

        public List<VertexPositionNormalTexture> Vertices
        {
            get { return this.mVertices; }
        }
        public List<Int32> Indicies
        {
            get { return this.mIndicies; }
        }

        public KryptonRenderHelper(GraphicsDevice graphicsDevice, Effect effect)
        {
            this.mGraphicsDevice = graphicsDevice;
            this.mEffect = effect;
        }

        public void BufferAddShadowHull(ShadowHull hull)
        {
            // Where are we in the buffer?
            var vertexCount = this.mVertices.Count;

            // Add the vertices to the buffer
            for (int i = 0; i < hull.NumVertices; i++)
            {
                var translatedVertex = hull.Vertices[i];

                // Create the matrix by which to transform the hull vertices
                Matrix hullMatrix = Matrix.CreateRotationZ(hull.Angle) * Matrix.CreateTranslation(hull.Position.X, hull.Position.Y, 0f);

                // Transform the vertices to screen coordinates
                Vector3.Transform(ref translatedVertex.Position, ref hullMatrix, out translatedVertex.Position);
                Vector3.TransformNormal(ref translatedVertex.Normal, ref hullMatrix, out translatedVertex.Normal);

                this.mVertices.Add(translatedVertex);
            }

            // Add the indicies to the buffer
            for (int i = 0; i < hull.NumIndicies; i++)
            {
                mIndicies.Add(vertexCount + hull.Indicies[i]);
            }

            // ----- SPEED TEST -----
            // Try some LINQ later to test speed ;)
            // mIndicies.AddRange(hull.Indicies.Select(x => x + vertexCount));
            // ----- SPEED TEST -----
        }

        public void DrawSquareQuad(Vector2 position, float rotation, float size, Color color)
        {
            rotation += (float)Math.PI / 4;

            var cos = (float)Math.Cos(rotation) * size;
            var sin = (float)Math.Sin(rotation) * size;

            var v1 = new Vector3(+cos, +sin, 0) + new Vector3(position, 0);
            var v2 = new Vector3(-sin, +cos, 0) + new Vector3(position, 0);
            var v3 = new Vector3(-cos, -sin, 0) + new Vector3(position, 0);
            var v4 = new Vector3(+sin, -cos, 0) + new Vector3(position, 0);

            var quad = new VertexPositionColorTexture[]
            {
                new VertexPositionColorTexture()
                {
                    Position = v2,
                    Color = color,
                    TextureCoordinate = new Vector2(0,0),
                },
                new VertexPositionColorTexture()
                {
                    Position = v1,
                    Color = color,
                    TextureCoordinate = new Vector2(1,0),
                },
                new VertexPositionColorTexture()
                {
                    Position = v3,
                    Color = color,
                    TextureCoordinate = new Vector2(0,1),
                },
                new VertexPositionColorTexture()
                {
                    Position = v4,
                    Color = color,
                    TextureCoordinate = new Vector2(1,1),
                },
            };

            this.mGraphicsDevice.DrawUserPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleStrip, quad, 0, 2);
        }

        public void BufferDraw()
        {
            if (this.mIndicies.Count >= 3)
            {
                this.mGraphicsDevice.DrawUserIndexedPrimitives<VertexPositionNormalTexture>(PrimitiveType.TriangleList, this.mVertices.ToArray(), 0, this.mVertices.Count, this.mIndicies.ToArray(), 0, this.mIndicies.Count / 3);
            }
        }

        internal void DrawFullscreenQuad()
        {
            // Obtain the original rendering states
            var originalRasterizerState = this.mGraphicsDevice.RasterizerState;

            // Draw the quad
            this.mEffect.CurrentTechnique = this.mEffect.Techniques["ScreenCopy"];
            //this.mGraphicsDevice.RasterizerState = RasterizerState.CullNone;

            this.mEffect.Parameters["TexelBias"].SetValue(new Vector2(0.5f / this.mGraphicsDevice.Viewport.Width, 0.5f / this.mGraphicsDevice.Viewport.Height));

            foreach (var effectPass in this.mEffect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                this.mGraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, KryptonRenderHelper.UnitQuad, 0, 2);
            }

            // Reset to the original rendering states
            //this.mGraphicsDevice.RasterizerState = originalRasterizerState;
        }

        public void BlurTextureToTarget(Texture2D texture, BlurTechnique blur)
        {
            // Get the pass to use
            string passName = "";

            switch (blur)
            {
                case (BlurTechnique.Horizontal):
                    passName = "HorizontalBlur";
                    break;

                case (BlurTechnique.Vertical):
                    passName = "VerticalBlur";
                    break;
            }

            // Calculate the texel bias
            Vector2 texelBias = new Vector2()
            {
                X = 0.5f / this.mGraphicsDevice.ScissorRectangle.Width,
                Y = 0.5f / this.mGraphicsDevice.ScissorRectangle.Height,
            };

            this.mEffect.Parameters["Texture0"].SetValue(texture);
            this.mEffect.Parameters["TexelBias"].SetValue(texelBias);
            this.mEffect.CurrentTechnique = this.mEffect.Techniques["Blur"];

            mEffect.CurrentTechnique.Passes[passName].Apply();
            this.mGraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, KryptonRenderHelper.UnitQuad, 0, 2);
        }

        public void DrawTextureToTarget(Texture2D texture, BlendTechnique blend)
        {
            // Get the technique to use
            string techniqueName = "";

            switch (blend)
            {
                case (BlendTechnique.Add):
                    techniqueName = "TextureToTarget_Add";
                    break;

                case (BlendTechnique.Multiply):
                    techniqueName = "TextureToTarget_Multiply";
                    break;
            }

            // Calculate the texel bias
            Vector2 texelBias = new Vector2()
            {
                X = 0.5f / this.mGraphicsDevice.ScissorRectangle.Width,
                Y = 0.5f / this.mGraphicsDevice.ScissorRectangle.Height,
            };

            this.mEffect.Parameters["Texture0"].SetValue(texture);
            this.mEffect.Parameters["TexelBias"].SetValue(texelBias);
            this.mEffect.CurrentTechnique = this.mEffect.Techniques[techniqueName];

            // Draw the quad
            foreach (var effectPass in this.mEffect.CurrentTechnique.Passes)
            {
                effectPass.Apply();
                this.mGraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, KryptonRenderHelper.UnitQuad, 0, 2);
            }
        }
    }
}