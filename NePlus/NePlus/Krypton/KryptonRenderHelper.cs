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
        private List<Int16> mIndicies = new List<Int16>();

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
        public List<Int16> Indicies
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
            // Why do we need all of these again? (hint: we don't)
            Matrix matrixRotation;
            Matrix matrixTranslation;
            Matrix matrixScale;
            Matrix matrixScaleInv;

            Matrix vertexMatrix;
            Matrix normalMatrix;

            // Where are we in the buffer?
            var vertexCount = this.mVertices.Count;

            // Add the vertices to the buffer
            foreach (var vertex in hull.Vertices)
            {
                var translatedVertex = vertex;

                matrixRotation = Matrix.CreateRotationZ(hull.Angle);
                matrixTranslation = Matrix.CreateTranslation(hull.Position.X, hull.Position.Y, 0f);
                matrixScale = Matrix.CreateScale(hull.Scale.X, hull.Scale.Y, 0);
                matrixScaleInv = Matrix.CreateScale(1f / hull.Scale.X, 1f / hull.Scale.Y, 0);

                // Create the matricies by which to transform the hull vertices and thier normals
                // ----- FIX THIS -----
                // This desperately needs to be optimized, as it would result in a major speed boost.
                vertexMatrix = matrixScale * matrixRotation * matrixTranslation;
                normalMatrix = matrixScaleInv * matrixRotation;

                // Transform the vertices to screen coordinates
                Vector3.Transform(ref translatedVertex.Position, ref vertexMatrix, out translatedVertex.Position);
                Vector3.TransformNormal(ref translatedVertex.Normal, ref normalMatrix, out translatedVertex.Normal);

                this.mVertices.Add(translatedVertex);
            }

            // Add the indicies to the buffer
            foreach (int index in hull.Indicies)
            {
                mIndicies.Add((Int16)(vertexCount + index));
            }
        }

        public void DrawSquareQuad(Vector2 position, float rotation, float size, Color color)
        {
            size /= 2;

            size = (float)Math.Sqrt(Math.Pow(size, 2) + Math.Pow(size, 2));

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

        public void DrawClippedFov(Vector2 position, float rotation, float size, Color color, float fov)
        {
            fov = MathHelper.Clamp(fov, 0, MathHelper.TwoPi);

            if (fov == 0)
            {
                return;
            }
            else if (fov == MathHelper.TwoPi)
            {
                this.DrawSquareQuad(position, rotation, size, color);
                return;
            }
            else
            {
                var ccw = ClampToBox(fov / 2);
                var cw = ClampToBox(-fov / 2);

                var ccwTex = new Vector2(ccw.X+1, -ccw.Y+1) / 2f;
                var cwTex = new Vector2(cw.X+1, -cw.Y+1) / 2f;

                //ccwTex = Vector2.Zero;
                //cwTex = Vector2.Zero;

                VertexPositionColorTexture[] vertices;

                #region Vertices
                vertices = new VertexPositionColorTexture[]
            {
                new VertexPositionColorTexture()
                {
                    Position = Vector3.Zero,
                    Color = color,
                    TextureCoordinate = new Vector2(0.5f, 0.5f),
                },
                new VertexPositionColorTexture()
                {
                    Position = new Vector3(ccw,0),
                    Color = color,
                    TextureCoordinate = ccwTex
                },
                new VertexPositionColorTexture()
                {
                    Position = new Vector3(-1, 1, 0),
                    Color = color,
                    TextureCoordinate = new Vector2(0, 0),
                },
                new VertexPositionColorTexture()
                {
                    Position = new Vector3(1, 1, 0),
                    Color = color,
                    TextureCoordinate = new Vector2(1, 0),
                },
                new VertexPositionColorTexture()
                {
                    Position = new Vector3(1, -1, 0),
                    Color = color,
                    TextureCoordinate = new Vector2(1, 1),
                },
                new VertexPositionColorTexture()
                {
                    Position = new Vector3(-1, -1, 0),
                    Color = color,
                    TextureCoordinate = new Vector2(0, 1),
                },
                new VertexPositionColorTexture()
                {
                    Position = new Vector3(cw, 0),
                    Color = color,
                    TextureCoordinate = cwTex,
                },
            };

                var r = Matrix.CreateRotationZ(rotation) * Matrix.CreateScale(size / 2) * Matrix.CreateTranslation(new Vector3(position, 0));

                for (int i = 0; i < vertices.Length; i++)
                {
                    var vertex = vertices[i];

                    Vector3.Transform(ref vertex.Position, ref r, out vertex.Position);

                    vertices[i] = vertex;
                }

                #endregion Vertices

                Int16[] indicies;

                #region Indicies

                if (fov == 0)
                {
                    indicies = new Int16[] { };
                }
                else if (fov <= MathHelper.Pi / 2)
                {
                    indicies = new Int16[]
                {
                    0, 1, 6,
                };
                }
                else if (fov <= 3 * MathHelper.Pi / 2)
                {
                    indicies = new Int16[]
                {
                    0, 1, 3,
                    0, 3, 4,
                    0, 4, 6,
                };
                }
                else
                {
                    indicies = new Int16[]
                {
                    0, 1, 2,
                    0, 2, 3,
                    0, 3, 4,
                    0, 4, 5,
                    0, 5, 6,
                };
                }
                #endregion Indicies

                this.mGraphicsDevice.DrawUserIndexedPrimitives<VertexPositionColorTexture>(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indicies, 0, indicies.Length / 3);
            }
        }

        public static Vector2 ClampToBox(float angle)
        {
            var x = Math.Cos(angle);
            var y = Math.Sin(angle);
            var absMax = Math.Max(Math.Abs(x), Math.Abs(y));

            return new Vector2((float)(x / absMax), (float)(y / absMax));
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

        public void BlurTextureToTarget(Texture2D texture, LightMapSize mapSize, BlurTechnique blur)
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

            var biasFactor = KryptonRenderHelper.BiasFactorFromLightMapSize(mapSize);

            // Calculate the texel bias
            Vector2 texelBias = new Vector2()
            {
                X = biasFactor / this.mGraphicsDevice.ScissorRectangle.Width,
                Y = biasFactor / this.mGraphicsDevice.ScissorRectangle.Height,
            };

            this.mEffect.Parameters["Texture0"].SetValue(texture);
            this.mEffect.Parameters["TexelBias"].SetValue(texelBias);
            this.mEffect.CurrentTechnique = this.mEffect.Techniques["Blur"];

            mEffect.CurrentTechnique.Passes[passName].Apply();
            this.mGraphicsDevice.DrawUserPrimitives<VertexPositionTexture>(PrimitiveType.TriangleStrip, KryptonRenderHelper.UnitQuad, 0, 2);
        }

        public void DrawTextureToTarget(Texture2D texture, LightMapSize mapSize, BlendTechnique blend)
        {
            // Get the technique to use
            string techniqueName = "";

            switch (blend)
            {
                case(BlendTechnique.Add):
                    techniqueName = "TextureToTarget_Add";
                    break;

                case(BlendTechnique.Multiply):
                    techniqueName = "TextureToTarget_Multiply";
                    break;
            }

            var biasFactor = KryptonRenderHelper.BiasFactorFromLightMapSize(mapSize);

            // Calculate the texel bias
            Vector2 texelBias = new Vector2()
            {
                X = biasFactor / this.mGraphicsDevice.ScissorRectangle.Width,
                Y = biasFactor / this.mGraphicsDevice.ScissorRectangle.Height,
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

        private static float BiasFactorFromLightMapSize(LightMapSize mapSize)
        {
            switch (mapSize)
            {
                case (LightMapSize.Full):
                    return 0.5f;

                case (LightMapSize.Fourth):
                    return 0.75f;

                case (LightMapSize.Eighth):
                    return 0.875f;

                default:
                    return 0.0f;
            }
        }
    }
}