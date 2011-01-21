using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NePlus.Krypton
{
    /// <summary>
    /// A hull used for casting shadows from a light source
    /// </summary>
    public class ShadowHull
    {
        #region Orientation

        /// <summary>
        /// The position of the shadow hull
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The angle of the shadow hull
        /// </summary>
        public float Angle;

        #endregion

        #region Shape

        /// <summary>
        /// The maximum radius in which all of the shadow hull's vertices are contained, originating from the hull's position
        /// </summary>
        public float MaxRadius;

        /// <summary>
        /// The vertices comprising the shadow hull
        /// </summary>
        public VertexPositionNormalTexture[] Vertices;

        /// <summary>
        /// The number of vertices comprising the shadow hull
        /// </summary>
        public int NumVertices;

        /// <summary>
        /// The indicies used to render the shadow hull
        /// </summary>
        public Int32[] Indicies;

        /// <summary>
        /// The number of indicies used to render the shadow hull
        /// </summary>
        public int NumIndicies;

        #endregion

        #region Constructor

        private ShadowHull()
        {
        }

        #endregion

        #region Factory Methods

        /// <summary>
        /// Creates a rectangular shadow hull
        /// </summary>
        /// <param name="size">The dimensions of the rectangle</param>
        /// <returns>A rectangular shadow hull</returns>
        public static ShadowHull CreateRectangle(Vector2 size)
        {
            ShadowHull hull = new ShadowHull();

            size *= 0.5f;

            hull.MaxRadius = (float)Math.Sqrt(size.X * size.X + size.Y * size.Y);

            hull.NumVertices = 4 * 2;
            var numTris = hull.NumVertices - 2;
            hull.NumIndicies = numTris * 3;

            hull.Vertices = new VertexPositionNormalTexture[hull.NumVertices];
            hull.Indicies = new Int32[hull.NumIndicies];

            // Vertex position
            var posTR = new Vector3(+size.X, +size.Y, 0f);
            var posBR = new Vector3(+size.X, -size.Y, 0f);
            var posBL = new Vector3(-size.X, -size.Y, 0f);
            var posTL = new Vector3(-size.X, +size.Y, 0f);

            // Vertex texture coordinates
            var texTR = new Vector2(1, 0);
            var texBR = new Vector2(1, 1);
            var texBL = new Vector2(0, 1);
            var texTL = new Vector2(0, 0);

            // Right
            hull.Vertices[0] = new VertexPositionNormalTexture(posTR, Vector3.Right, texTR);
            hull.Vertices[1] = new VertexPositionNormalTexture(posBR, Vector3.Right, texBR);

            // Bottom
            hull.Vertices[2] = new VertexPositionNormalTexture(posBR, Vector3.Down, texBR);
            hull.Vertices[3] = new VertexPositionNormalTexture(posBL, Vector3.Down, texBL);

            // Left
            hull.Vertices[4] = new VertexPositionNormalTexture(posBL, Vector3.Left, texBL);
            hull.Vertices[5] = new VertexPositionNormalTexture(posTL, Vector3.Left, texTL);

            // Top
            hull.Vertices[6] = new VertexPositionNormalTexture(posTL, Vector3.Up, texTL);
            hull.Vertices[7] = new VertexPositionNormalTexture(posTR, Vector3.Up, texTR);

            // Create tris
            for (int i = 0; i < numTris; i++)
            {
                hull.Indicies[i * 3 + 0] = 0;
                hull.Indicies[i * 3 + 1] = i + 1;
                hull.Indicies[i * 3 + 2] = i + 2;
            }

            return hull;
        }

        /// <summary>
        /// Creates a circular shadow hull
        /// </summary>
        /// <param name="radius">radius of the circle</param>
        /// <param name="sides">number of sides the circle will be comprised of</param>
        /// <returns>A circular shadow hull</returns>
        public static ShadowHull CreateCircle(float radius, int sides)
        {
            // Validate input
            if (sides < 3) { throw new ArgumentException("Shadow hull must have at least 3 sides."); }

            ShadowHull hull = new ShadowHull();

            hull.MaxRadius = radius;

            // Calculate number of sides
            hull.NumVertices = sides * 2;
            var numTris = hull.NumVertices - 2;
            hull.NumIndicies = numTris * 3;

            hull.Vertices = new VertexPositionNormalTexture[hull.NumVertices];
            hull.Indicies = new Int32[hull.NumIndicies];

            var angle = (float)(-Math.PI * 2) / sides; // XNA Renders Clockwise
            var angleOffset = angle / 2;

            for (int i = 0; i < sides; i++)
            {
                // Create vertices
                var v1 = new VertexPositionNormalTexture();
                var v2 = new VertexPositionNormalTexture();

                // Vertex Position
                v1.Position.X = (float)Math.Cos(angle * i) * radius;
                v1.Position.Y = (float)Math.Sin(angle * i) * radius;
                v1.Position.Z = 0;

                v2.Position.X = (float)Math.Cos(angle * (i + 1)) * radius;
                v2.Position.Y = (float)Math.Sin(angle * (i + 1)) * radius;
                v2.Position.Z = 0;

                // Vertex Normal
                v1.Normal.X = (float)Math.Cos(angle * i + angleOffset);
                v1.Normal.Y = (float)Math.Sin(angle * i + angleOffset);
                v1.Normal.Z = 0;

                v2.Normal.X = (float)Math.Cos(angle * i + angleOffset);
                v2.Normal.Y = (float)Math.Sin(angle * i + angleOffset);
                v2.Normal.Z = 0;

                // Vertex Texture Coordinates
                v1.TextureCoordinate.X = +v1.Position.X;
                v1.TextureCoordinate.Y = -v1.Position.Y;

                v2.TextureCoordinate.X = +v2.Position.X;
                v2.TextureCoordinate.Y = -v2.Position.Y;

                // Copy vertices
                hull.Vertices[i * 2 + 0] = v1;
                hull.Vertices[i * 2 + 1] = v2;
            }

            for (int i = 0; i < numTris; i++)
            {
                hull.Indicies[i * 3 + 0] = 0;
                hull.Indicies[i * 3 + 1] = (Int16)(i + 1);
                hull.Indicies[i * 3 + 2] = (Int16)(i + 2);
            }

            return hull;
        }

        /// <summary>
        /// Creates a custom shadow hull based on a series of vertices
        /// </summary>
        /// <param name="points">The points which the shadow hull will be comprised of</param>
        /// <returns>A custom shadow hulll</returns>
        public static ShadowHull CreateConvex(ref Vector2[] points)
        {
            // Validate input
            if (points == null) { throw new ArgumentNullException("Points cannot be null."); }
            if (points.Length < 3) { throw new ArgumentException("Need at least 3 points to create shadow hull."); }

            var numPoints = points.Length;

            ShadowHull hull = new ShadowHull();

            hull.NumVertices = numPoints * 2;
            var numTris = hull.NumVertices - 2;
            hull.NumIndicies = numTris * 3;

            hull.Vertices = new VertexPositionNormalTexture[hull.NumVertices];
            hull.Indicies = new Int32[hull.NumIndicies];

            Vector2 pointMin = points[0];
            Vector2 pointMax = points[0];

            for (int i = 1; i < numPoints; i++)
            {
                var point = points[i];

                // These mins and max's could be optimised, but it's probably not worth it.
                pointMin.X = Math.Min(pointMin.X, point.X);
                pointMin.Y = Math.Min(pointMin.Y, point.Y);

                pointMax.X = Math.Max(pointMin.X, point.X);
                pointMax.Y = Math.Max(pointMin.Y, point.Y);
            }

            var texSize = pointMax - pointMin;

            for (int i = 0; i < numPoints; i++)
            {
                var p1 = points[(i + 0) % numPoints];
                var p2 = points[(i + 1) % numPoints];

                hull.MaxRadius = Math.Max(hull.MaxRadius, p1.Length());

                var line = p2 - p1;
                var normal = new Vector2(-line.Y, +line.X);

                normal.Normalize();

                // Generate texture coordinates
                var tex1 = (p1 - pointMin) / texSize;
                var tex2 = (p2 - pointMin) / texSize;

                // Flip texture coordinates vertically
                tex1.Y = 1 - tex1.Y;
                tex2.Y = 1 - tex2.Y;

                var v1 = new VertexPositionNormalTexture()
                {
                    Position = new Vector3(p1, 0f),
                    Normal = new Vector3(normal, 0f),
                    TextureCoordinate = tex1,
                };

                var v2 = new VertexPositionNormalTexture()
                {
                    Position = new Vector3(p2, 0f),
                    Normal = new Vector3(normal, 0f),
                    TextureCoordinate = tex2,
                };

                hull.Vertices[i * 2 + 0] = v1;
                hull.Vertices[i * 2 + 1] = v2;
            }

            for (Int16 i = 0; i < numTris; i++)
            {
                hull.Indicies[i * 3 + 0] = 0;
                hull.Indicies[i * 3 + 1] = (Int16)(i + 1);
                hull.Indicies[i * 3 + 2] = (Int16)(i + 2);
            }

            return hull;
        }

        #endregion
    }
}