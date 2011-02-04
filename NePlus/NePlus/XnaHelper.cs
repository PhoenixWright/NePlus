using System;
using System.Collections.Generic;

using Microsoft.Xna.Framework;

namespace NePlus
{
    public static class XnaHelper
    {
        /// <summary>
        /// Determines whether or not a point is inside a triangle.
        /// </summary>
        /// <param name="TrianglePoints">A list of Vector2s comprising a triangle.</param>
        /// <param name="p">The point to confirm or deny is in the triangle.</param>
        /// <returns>True if the point is in the triangle, false if not.</returns>
        public static bool IsPointInsideTriangle(List<Vector2> TrianglePoints, Vector2 p)
        {
            Vector2 e0 = p - TrianglePoints[0];
            Vector2 e1 = TrianglePoints[1] - TrianglePoints[0];
            Vector2 e2 = TrianglePoints[2] - TrianglePoints[0];

            float u, v = 0;
            if (e1.X == 0)
            {
                if (e2.X == 0) return false;
                u = e0.X / e2.X;
                if (u < 0 || u > 1) return false;
                if (e1.Y == 0) return false;
                v = (e0.Y - e2.Y * u) / e1.Y;
                if (v < 0) return false;
            }
            else
            {
                float d = e2.Y * e1.X - e2.X * e1.Y;
                if (d == 0) return false;
                u = (e0.Y * e1.X - e0.X * e1.Y) / d;
                if (u < 0 || u > 1) return false;
                v = (e0.X - e2.X * u) / e1.X;
                if (v < 0) return false;
                if ((u + v) > 1) return false;
            }

            return true;
        }

        /// <summary>
        /// Rotates a Vector2 around a point a given amount of radians.
        /// </summary>
        /// <param name="point">The point to rotate.</param>
        /// <param name="radians">The amount to rotate the point.</param>
        /// <param name="pivot">The point to rotate around.</param>
        /// <returns>The rotated point.</returns>
        public static Vector2 RotateVector2(Vector2 point, float radians, Vector2 pivot)
        {
            float cosRadians = (float)Math.Cos(radians);
            float sinRadians = (float)Math.Sin(radians);

            Vector2 translatedPoint = new Vector2();
            translatedPoint.X = point.X - pivot.X;
            translatedPoint.Y = point.Y - pivot.Y;

            Vector2 rotatedPoint = new Vector2();
            rotatedPoint.X = translatedPoint.X * cosRadians - translatedPoint.Y * sinRadians + pivot.X;
            rotatedPoint.Y = translatedPoint.X * sinRadians + translatedPoint.Y * cosRadians + pivot.Y;

            return rotatedPoint;
        }
    }
}
