using System;

using Microsoft.Xna.Framework;

namespace NePlus
{
    public static class XnaHelper
    {
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
