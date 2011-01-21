using System;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NePlus.Krypton
{
    public static class LightTextureBuilder
    {

        public static Texture2D CreatePointLight(GraphicsDevice device, int size)
        {
            return CreateConicLight(device, size, MathHelper.TwoPi, 0);
        }

        public static Texture2D CreateConicLight(GraphicsDevice device, int size, float FOV)
        {
            return CreateConicLight(device, size, FOV, 0);
        }

        public static Texture2D CreateConicLight(GraphicsDevice device, int size, float FOV, float nearPlaneDistance)
        {
            /*if (!IsPowerOfTwo(size))
                throw new Exception("The size must be a power of 2");*/
            float[,] Data = new float[size, size];

            float center = size / 2;

            FOV = FOV / 2;

            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                {
                    float Distance = Vector2.Distance(new Vector2(x, y), new Vector2(center));

                    Vector2 Difference = new Vector2(x, y) - new Vector2(center);
                    float Angle = (float)Math.Atan2(Difference.Y, Difference.X);

                    if (Distance <= center && Distance >= nearPlaneDistance && Math.Abs(Angle) <= FOV)
                        Data[x, y] = (center - Distance) / center;
                    else
                        Data[x, y] = 0;

                    Data[x, y] *= Data[x, y];
                }

            Texture2D tex = new Texture2D(device, size, size);

            Color[] Data1D = new Color[size * size];
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    Data1D[x + y * size] = new Color(new Vector3(Data[x, y]));

            tex.SetData<Color>(Data1D);

            return tex;
        }

        private static bool IsPowerOfTwo(int x)
        {
            return (x & (x - 1)) == 0;
        }
    }
}